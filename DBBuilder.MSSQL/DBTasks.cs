using System;
using System.Configuration;
using DBBuilder.MSSQL.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace DBBuilder.MSSQL
{
	/// <summary>
	/// Base class for all DB-related MSBuild tasks
	/// </summary>
	public abstract class DBTask : Task
	{
		private string _serverName;
		/// <summary>
		/// Name of the server to connect to
		/// </summary>
		[Required]
		public string ServerName
		{
			get { return this._serverName; }
			set { this._serverName = value; }
		}

		private string _databaseName;
		/// <summary>
		/// Name of the database to work with
		/// </summary>
		[Required]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		internal static TraceSwitch traceSwitch = new TraceSwitch("DBTasksTraceLevel", "Level of trace messages to show");
		static DBTask()
		{
			string dllFileName = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
			if (File.Exists(dllFileName + ".config"))
			{
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(dllFileName);
				string[] configKeys = config.AppSettings.Settings.AllKeys;
				if (Array.Exists<string>(configKeys, delegate(string s) { return (s == "DBTasksTraceLevel") ? true : false; }))
				{
					string traceLevelVal = config.AppSettings.Settings["DBTasksTraceLevel"].Value;
					if (traceLevelVal.Length > 0)
					{
						int value;
						if (int.TryParse(traceLevelVal, out value))
						{
							value = value > 4 ? 4 : value;
							traceSwitch.Level = (TraceLevel)value;
						}
						else if (Enum.IsDefined(typeof(TraceLevel), traceLevelVal))
						{
							traceSwitch.Level = (TraceLevel)Enum.Parse(typeof(TraceLevel), traceLevelVal);
						}
					}
				}
			}
			Trace.AutoFlush = true;
			Trace.Listeners.Add(new ConsoleTraceListener());
		}

	}

	/// <summary>
	/// Database task that deals with objects interdependencies through dependency file
	/// </summary>
	public abstract class DBTaskWithDependency : DBTask
	{
		private string _dependencyFileName;

		/// <summary>
		/// Name of the dependency file.
		/// </summary>
		[Required]
		public string DependencyFileName
		{
			get { return _dependencyFileName; }
			set { _dependencyFileName = value; }
		}
	}

	/// <summary>
	/// Re-creates extended database objects from scripts stored locally (and created by DBScriptTask).
	/// </summary>
	public class DBUpgradeTask : DBTaskWithDependency
	{
		private string _conversionDirName;
		public string ConversionDirName
		{
			get { return _conversionDirName; }
			set { _conversionDirName = value; }
		}

		private string _sourceDirName = "";
		public string SourceDirName
		{
			get { return _sourceDirName; }
			set { _sourceDirName = value; }
		}

		private bool _lockDatabase = true;
		public bool LockDatabase
		{
			get { return _lockDatabase; }
			set { _lockDatabase = value; }
		}

		public override bool Execute()
		{
			Trace.WriteIf(traceSwitch.TraceVerbose, "DBUpgradeTask invoked");
			SqlServerHelper.UpgradeDB(ServerName, DatabaseName, SourceDirName, DependencyFileName, ConversionDirName, LockDatabase, this.Log);
			return !this.Log.HasLoggedErrors;
		}
	}

	/// <summary>
	/// Creates new database from scripts
	/// </summary>
	public class DBCreateNewTask : DBTaskWithDependency
	{
		private string _sourceDirName = "";
		[Required]
		public string SourceDirName
		{
			get { return _sourceDirName; }
			set { _sourceDirName = value; }
		}

		public override bool Execute()
		{
			Trace.WriteIf(traceSwitch.TraceVerbose, "DBCreateNewTask invoked");
			SqlServerHelper.CreateNewDB(ServerName, DatabaseName, SourceDirName, DependencyFileName, false, "", this.Log);
			return !this.Log.HasLoggedErrors;
		}
	}

	/// <summary>
	/// Restore database objects using dependency file
	/// </summary>
	public class DBRestoreObjectsTask : DBTaskWithDependency
	{
		private string _sourceDirName = "";
		[Required]
		public string SourceDirName
		{
			get { return _sourceDirName; }
			set { _sourceDirName = value; }
		}

		private bool _lockDatabase = false;
		public bool LockDatabase
		{
			get { return _lockDatabase; }
			set { _lockDatabase = value; }
		}

		private bool _dropExistingObjectsOfTheSameType = true;
		public bool DropExistingObjectsOfTheSameType
		{
			get { return _dropExistingObjectsOfTheSameType; }
			set { _dropExistingObjectsOfTheSameType = value; }
		}


		public override bool Execute()
		{
			Trace.WriteIf(traceSwitch.TraceVerbose, "DBRestoreObjectsTask invoked");
			SqlServerHelper.RestoreDBObjects(ServerName, DatabaseName, SourceDirName, DependencyFileName, 
			                                 LockDatabase, DropExistingObjectsOfTheSameType, this.Log);
			return !this.Log.HasLoggedErrors;
		}
	}

	/// <summary>
	/// This task is used for scripting entire database and store scripts to filesystem.
	/// </summary>
	public class DBScriptTask : DBTaskWithDependency
	{
		private const SqlObjectType logicTypes = SqlObjectType.StoredProcedure | SqlObjectType.Trigger | SqlObjectType.UserDefinedFunction | SqlObjectType.Assembly;
		private const SqlObjectType dataTypes = SqlObjectType.UserTable | SqlObjectType.View | SqlObjectType.FTCatalog;
		private const SqlObjectType securityTypes = SqlObjectType.Schema;

		private string _destinationDirName = "";
		[Required]
		public string DestinationDir
		{
			get { return _destinationDirName; }
			set { _destinationDirName = value; }
		}

		private string _objectsToScript = "";
		[Required]
		public string ObjectsToScript
		{
			get { return _objectsToScript; }
			set { _objectsToScript = value; }
		}

		public override bool Execute()
		{
			Trace.WriteIf(traceSwitch.TraceVerbose, "DBScriptTask invoked");

			SqlObjectType objectTypes = ParseObjectTypeString(ObjectsToScript);

			SqlServerHelper.ScriptDB(ServerName, DatabaseName, DestinationDir, DependencyFileName,
			                         objectTypes,
			                         ObjectSaveOptions.AddObjectTypePrefix | ObjectSaveOptions.SaveEachObjectTypeToSeparateDir,
			                         this.Log);

			return !this.Log.HasLoggedErrors;
		}

		private SqlObjectType ParseObjectTypeString(string objectTypes)
		{
			SqlObjectType result = SqlObjectType.None;
			string[] selectedObjectTypes = objectTypes.Split('|');
			foreach (string objectType in selectedObjectTypes)
			{
				if (Enum.IsDefined(typeof(SqlObjectType), objectType))
				{
					result = result | (SqlObjectType)Enum.Parse(typeof(SqlObjectType), objectType);
				}
				else
				{
					switch (objectType)
					{
						case "Data":
							result = result | dataTypes;
							break;
						case "Logic":
							result = result | logicTypes;
							break;
						case "Security":
							result = result | securityTypes;
							break;
						case "All":
							result = result | dataTypes | logicTypes | securityTypes;
							break;
							//default:
							//   result = result | logicTypes;
							//   break;
					}
				}
			}
			return result;
		}
	}

	/// <summary>
	/// Scripts table data.
	/// </summary>
	public class DBScriptDataTask : DBTaskWithDependency
	{
		private string _destinationDirName = "";
		[Required]
		public string DestinationDir
		{
			get { return _destinationDirName; }
			set { _destinationDirName = value; }
		}

		private string _tableMask = "%";
		/// <summary>
		/// Mask to filter tables to script data from. This mask is later use in LIKE operator.
		/// Default value is '%' (i.e. 'all tables'). You can separate different masks by using semicolon
		/// (e.g. 'dic%;tblTask%').
		/// </summary>
		public string TableMask
		{
			get { return _tableMask; }
			set { _tableMask = value; }
		}

		public override bool Execute()
		{
			Trace.WriteIf(traceSwitch.TraceVerbose, "DBScriptDataTask invoked");
			SqlServerHelper.ScriptDBData(ServerName, DatabaseName, DestinationDir, DependencyFileName, TableMask,
			                             DataSaveOptions.None, this.Log);

			return true;
		}
	}

	public class DBClearTask : DBTask
	{

		public override bool Execute()
		{
			Trace.WriteIf(traceSwitch.TraceVerbose, "DBClearTask invoked");
			SqlServerHelper.ClearDB(ServerName, DatabaseName, Log);
			return !Log.HasLoggedErrors;
		}
	}
}