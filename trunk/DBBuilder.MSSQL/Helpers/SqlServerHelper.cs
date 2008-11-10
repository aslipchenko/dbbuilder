using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DBBuilder.MSSQL;
using System.IO;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using Microsoft.Build.Utilities;
using System.Collections;
using System.Diagnostics;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DBBuilder.MSSQL.Helpers
{
	/// <summary>
	/// Functions for working with SQL Server SMO
	/// </summary>
	public class SqlServerHelper
	{

		#region Public members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="databaseName"></param>
		/// <param name="outputDir"></param>
		/// <param name="dependencyFileName"></param>
		/// <param name="objectTypes"></param>
		/// <param name="saveOptions"></param>
		/// <param name="logger"></param>
		public static void ScriptDB(string serverName, string databaseName,
		                            string outputDir,
		                            string dependencyFileName,
		                            SqlObjectType objectTypes,
		                            ObjectSaveOptions saveOptions,
		                            TaskLoggingHelper logger)
		{

			if (dependencyFileName == string.Empty)
			{
				throw new InvalidArgumentException(Resources.msgNoDependencyFileName);
			}

			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgScriptDBInvocation, 
			                                databaseName, outputDir, dependencyFileName, objectTypes.ToString()));

			Server srv = ConnectToServer(serverName, databaseName);

			ScriptingOptions dropOptions = new ScriptingOptions();
			dropOptions.AppendToFile = false;
			dropOptions.ToFileOnly = true;
			dropOptions.IncludeIfNotExists = true;
			dropOptions.ScriptDrops = true;
			dropOptions.Encoding = Encoding.UTF8;
			dropOptions.AnsiFile = true;

			ScriptingOptions createOptions = new ScriptingOptions();
			createOptions.AppendToFile = true;
			createOptions.ToFileOnly = true;
			createOptions.ClusteredIndexes = true;
			createOptions.Indexes = true;
			createOptions.DriAll = true;
			createOptions.NoCollation = true;
			createOptions.SchemaQualifyForeignKeysReferences = true;
			createOptions.Encoding = Encoding.UTF8;
			createOptions.AnsiFile = true;

			// InitializeDependencyTable();
			// ToDo: add more fields to support more object types
			DataTable dependencies = InitializeDependencyTable();

			PrepareOutputDir(outputDir, objectTypes, saveOptions);

			if ((objectTypes & SqlObjectType.Schema) > 0 & srv.Information.Version.Major > 8)
			{
				foreach (Schema s in srv.Databases[databaseName].Schemas)
				{
					if (s.ID > 4 && s.ID < 0x4000)
					{
						ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, s.Name, s, SqlObjectType.Schema, logger);
					}
				}
			}

			if ((objectTypes & SqlObjectType.FTCatalog) > 0)
			{
				dropOptions.FullTextIndexes = false;
				createOptions.FullTextCatalogs = true;
				foreach (FullTextCatalog ftc in srv.Databases[databaseName].FullTextCatalogs)
				{
					ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, ftc.Name, ftc, SqlObjectType.FTCatalog, logger);
				}
				createOptions.FullTextIndexes = true;
			}

			if ((objectTypes & (SqlObjectType.UserTable | SqlObjectType.Trigger)) > 0)
			{
				foreach (Table t in srv.Databases[databaseName].Tables)
				{
					// for some strange reason system tables are here too ...
					if ((objectTypes & SqlObjectType.UserTable) > 0 & !t.IsSystemObject)
					{
						ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, t.Name, t, SqlObjectType.UserTable, logger);
					}

					if ((objectTypes & SqlObjectType.Trigger) > 0)
					{
						foreach (Trigger ttr in t.Triggers)
						{
							ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, ttr.Name, ttr, SqlObjectType.Trigger, logger);
						}
					}
				}
			}

			if ((objectTypes & SqlObjectType.View) > 0)
			{
				foreach (View v in srv.Databases[databaseName].Views)
				{
					if (!v.IsSystemObject)
					{
						ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, v.Name, v, SqlObjectType.View, logger);
					}
				}
			}

			if ((objectTypes & SqlObjectType.Assembly) > 0 & srv.Information.Version.Major > 8)
			{
				foreach (SqlAssembly a in srv.Databases[databaseName].Assemblies)
				{
					ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, a.Name, a, SqlObjectType.Assembly, logger);
				}
			}


			if ((objectTypes & SqlObjectType.StoredProcedure) > 0)
			{
				DataTable spData = GetDependencies(SqlObjectType.StoredProcedure, srv);
				foreach (DataRow r in spData.Rows)
				{
					StoredProcedure sp = srv.Databases[databaseName].StoredProcedures[(string)r["oObjName"]];
					ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, sp.Name, sp, SqlObjectType.StoredProcedure, logger);
				}
				//foreach (StoredProcedure sp in srv.Databases[databaseName].StoredProcedures)
				//{
				//   if (!sp.IsSystemObject)
				//   {
				//      ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, sp.Name, sp, SqlObjectType.StoredProcedure, logger);
				//   }
				//}
			}

			if ((objectTypes & SqlObjectType.UserDefinedFunction) > 0)
			{
				DataTable spData = GetDependencies(SqlObjectType.UserDefinedFunction, srv);
				foreach (DataRow r in spData.Rows)
				{
					UserDefinedFunction sp = srv.Databases[databaseName].UserDefinedFunctions[(string)r["oObjName"]];
					// For some strange reason it is possible to recieve CLR Triggers in collection of UDFs 
					// returned by appropriate sp_MSDependencies call ... So we're doublechecking here
					if (sp != null)
					{
						ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, sp.Name, sp, SqlObjectType.UserDefinedFunction, logger);
					}
				}
				//foreach (UserDefinedFunction udf in srv.Databases[databaseName].UserDefinedFunctions)
				//{
				//   if (!udf.IsSystemObject)
				//   {
				//      ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, udf.Name, udf, SqlObjectType.UserDefinedFunction, logger);
				//   }
				//}
			}

			if ((objectTypes & SqlObjectType.Trigger) > 0 & srv.Information.Version.Major > 8)
			{
				foreach (Trigger tr in srv.Databases[databaseName].Triggers)
				{
					ScriptObject(outputDir, saveOptions, dependencies, dropOptions, createOptions, tr.Name, tr, SqlObjectType.Trigger, logger);
				}
			}

			UpdateDependencies(objectTypes, dependencies, srv);
			dependencies.WriteXml(outputDir + "\\" + dependencyFileName);

		}


		/// <summary>
		/// Upgrades given database by applying conversion scripts from supplied directory (conversionDirName)
		/// and restoring database objects (SPs, UDFs, etc.) from supplied location (inputDirName)
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="databaseName">Name of the database to be upgraded</param>
		/// <param name="inputDirName">Directory that contains </param>
		/// <param name="dependencyFileName">Name of the file with objects interdependency info</param>
		/// <param name="conversionDirName">Directory that contains conversion scripts to be applied</param>
		/// <param name="lockDatabase">Weather the database should be put to single-user mode while upgrading</param>
		/// <param name="logger">Callback to pass log messages to caller</param>
		public static void UpgradeDB(string serverName, string databaseName,
		                             string inputDirName,
		                             string dependencyFileName,
		                             string conversionDirName,
		                             bool lockDatabase,
		                             TaskLoggingHelper logger)
		{
			if (inputDirName == string.Empty && conversionDirName == string.Empty)
			{
				throw new InvalidArgumentException("You should provide input directory name or conversion directory name");
			}

			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgUpgradeDBInvocation, 
			                                serverName, databaseName, inputDirName, dependencyFileName, conversionDirName, lockDatabase));

			Server srv = ConnectToServer(serverName, databaseName);

			if (lockDatabase)
			{
				srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlSingleUser, databaseName));
			}

			try
			{

				#region Determine which object types to drop

				DataTable _dependencies = ReadDependencyFile(inputDirName, dependencyFileName, logger);
				if (_dependencies == null)
				{
					return;
				}

				SqlObjectType objectTypesToDrop = GatherObjectTypes(_dependencies);

				#endregion

				// Drop all objects that we're supposed to create later
				DropDBObjects(srv, databaseName, objectTypesToDrop, logger);

				#region Conversion scripts

				if (conversionDirName != string.Empty)
				{
					MD5 md5 = new MD5CryptoServiceProvider();
					string[] conversionScriptList = Directory.GetFiles(conversionDirName, "*.sql");
					int i;
					for (i = 0; i < conversionScriptList.Length; i++)
					{
						string fileName = conversionScriptList[i];
						string fileContent = "";
						string md5Hash = "";

						try
						{
							fileContent = File.ReadAllText(fileName);
							md5Hash = GetHexString(md5.ComputeHash(Encoding.Unicode.GetBytes(fileContent)));

							// Check if this script was executed before
							SqlDataReader rdr = srv.ConnectionContext.ExecuteReader(string.Format(Resources.sqlFindConversionFileByCheckSum, md5Hash));
							if (!rdr.Read())
							{
								rdr.Close();
								srv.ConnectionContext.ExecuteNonQuery(fileContent);
								logger.LogMessage(string.Format(Resources.msgOK, fileName));

								SaveConversionResults(srv, md5Hash, fileName, fileContent, true, logger);
							}
							else
							{
								logger.LogMessage(string.Format(Resources.msgScriptWasExecutedBefore,
								                                fileName, rdr.GetInt32(0), rdr.GetDateTime(1)));
								rdr.Close();
							}
						}
						catch (ExecutionFailureException ex)
						{
							logger.LogError(string.Format(Resources.msgFailure, fileName, ex));
							logger.LogMessage(Resources.msgSkipConversionScriptsAfterError);
							SaveConversionResults(srv, md5Hash, fileName, fileContent, false, logger);
							break;
						}
						catch (IOException ex)
						{
							logger.LogError(string.Format(Resources.msgFailure, fileName, ex.Message));
						}
					} // for

					logger.LogMessage(string.Format(Resources.msgFinishedConversion, databaseName, i));

				}

				#endregion

				#region Restoring objects from files

				if (inputDirName != string.Empty)
				{
					CreateDBObjects(srv, inputDirName, dependencyFileName, logger);
				}

				#endregion
			}
			finally
			{
				if (lockDatabase)
				{
					srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlMultiUser, databaseName));
				}
			}

		}

		/// <summary>
		/// Read dependency data from file
		/// </summary>
		/// <param name="inputDirName">Base directory name</param>
		/// <param name="dependencyFileName">Dependency file name</param>
		/// <param name="logger"></param>
		/// <returns>Initialized DataTable or null in case of exception</returns>
		private static DataTable ReadDependencyFile(string inputDirName, string dependencyFileName, TaskLoggingHelper logger)
		{
			DataTable _dependencies = InitializeDependencyTable();
			try
			{
				_dependencies.ReadXml(inputDirName + @"\" + dependencyFileName);
			}
			catch (ConstraintException ex)
			{
				StringBuilder exceptionData = new StringBuilder();
				exceptionData.AppendLine(Resources.msgDependencyFileConstraintViolation);
				foreach (DictionaryEntry de in ex.Data)
				{
					exceptionData.AppendFormat("{0} = {1}", de.Key, de.Value);
				}
				logger.LogError(exceptionData.ToString());
				return null;
			}
			catch (IOException ex)
			{
				logger.LogError(ex.Message);
				return null;
			}
			return _dependencies;
		}

		/// <summary>
		/// Collects object types present in passed DataTable with dependency data
		/// </summary>
		/// <param name="_dependencies">Dependency DataTable</param>
		/// <returns></returns>
		private static SqlObjectType GatherObjectTypes(DataTable _dependencies)
		{
			List<string> objectTypesPresent = new List<string>();
			foreach (DataRow r in _dependencies.Rows)
			{
				string val = (r["oType"] as string);
				if (val != null && !objectTypesPresent.Contains(val))
				{
					objectTypesPresent.Add(val);
				}
			}

			SqlObjectType objectTypesToDrop = SqlObjectType.None;
			objectTypesPresent.ForEach(delegate(string val)
			                           	{
			                           		if (Enum.IsDefined(typeof(SqlObjectType), val))
			                           		{
			                           			objectTypesToDrop = objectTypesToDrop | (SqlObjectType)Enum.Parse(typeof(SqlObjectType), val);
			                           		}
			                           	});
			return objectTypesToDrop;
		}



		/// <summary>
		/// Drops all object of given types in target database on target server
		/// </summary>
		/// <param name="srv">Server object</param>
		/// <param name="databaseName">Database to use</param>
		/// <param name="objectTypes">Object type(s) to drop</param>
		/// <param name="logger"></param>
		private static void DropDBObjects(Server srv, string databaseName, SqlObjectType objectTypes, TaskLoggingHelper logger)
		{
			DataTable depData;

			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgDropDBObjectsInvocation, objectTypes.ToString()));

			bool processTriggers = (objectTypes & SqlObjectType.Trigger) > 0;

			foreach (object val in Enum.GetValues(typeof(SqlObjectType)))
			{
				int requiredServerVersion = GetRequiredServerVersion((SqlObjectType)val);
				if ((objectTypes & (SqlObjectType)val) > 0)
				{
					if (srv.Information.Version.Major >= requiredServerVersion)
					{
						depData = GetDependencies((SqlObjectType)val, srv);
						//sql = Resources.sqlMSDependencyTemplate + Enum.Format(typeof(SqlObjectType), val, "x");
						//depData = srv.ConnectionContext.ExecuteWithResults(sql).Tables[0];
						depData.DefaultView.Sort = "oSequence desc";

						for (int i = 0; i < depData.DefaultView.Count; i++)
						{
							string objectName = (string)depData.DefaultView[i]["oObjName"];
							object objectToDrop = GetDBObjectByName(srv, databaseName, objectName, (SqlObjectType)val);
							if ((objectToDrop != null) && (objectToDrop is IDroppable))
							{
								(objectToDrop as IDroppable).Drop();

								Trace.WriteLineIf(DBTask.traceSwitch.TraceVerbose,
								                  string.Format(Resources.traceMsgObjectDropped, objectName));
							}
						}

						logger.LogMessage(string.Format(Resources.msgDeletedObjects, val.ToString(), databaseName));
					}
					else
					{
						Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
						                  string.Format(Resources.traceMsgSkippingObjectTypeDueToLowServerVersion, 
						                                val.ToString(), requiredServerVersion));
					}
				}
			}

			// Special processing for table triggers (as these triggres reside under table object)
			// Ugly, but I don't see another way of doing this ...
			if (processTriggers)
			{
				foreach (Table t in srv.Databases[databaseName].Tables)
				{
					// We can not drop triggers in foreach (Trigger ttr in t.Triggers) loop as dropping a trigger
					// alters Triggers collection (thus preventing enumerator from working correctly
					List<int> triggerIDs = new List<int>();
					foreach (Trigger ttr in t.Triggers)
					{
						triggerIDs.Add(ttr.ID);
					}
					// Now we can drop them one by one (at last!)
					foreach (int id in triggerIDs)
					{
						Trigger tr = t.Triggers.ItemById(id);
						string triggerName = tr.Name;
						tr.Drop();

						Trace.WriteLineIf(DBTask.traceSwitch.TraceVerbose,
						                  string.Format(Resources.traceMsgObjectDropped, triggerName));
					}
				}
				logger.LogMessage(string.Format(Resources.msgDeletedObjects, "table triggers", databaseName));
			}
		}

		/// <summary>
		/// Facade for accessing different object types by type and name.
		/// </summary>
		/// <param name="srv">Server object</param>
		/// <param name="databaseName">Database to use</param>
		/// <param name="objectName">Name of the object to look for</param>
		/// <param name="objectType">Type of the object</param>
		/// <returns>Reference to the object or null if object type is not supported or object not found</returns>
		private static object GetDBObjectByName(Server srv, string databaseName, string objectName, SqlObjectType objectType)
		{
			object result = null;
			switch (objectType)
			{
				case SqlObjectType.UserDefinedFunction:
					result = srv.Databases[databaseName].UserDefinedFunctions[objectName];
					break;
				case SqlObjectType.View:
					result = srv.Databases[databaseName].Views[objectName];
					break;
				case SqlObjectType.UserTable:
					result = srv.Databases[databaseName].Tables[objectName];
					break;
				case SqlObjectType.StoredProcedure:
					result = srv.Databases[databaseName].StoredProcedures[objectName];
					break;
				case SqlObjectType.Trigger:
					result = srv.Databases[databaseName].Triggers[objectName];
					break;
				case SqlObjectType.Assembly:
					result = srv.Databases[databaseName].Assemblies[objectName];
					break;
				case SqlObjectType.UserDefinedType:
					result = srv.Databases[databaseName].UserDefinedTypes[objectName];
					break;
				case SqlObjectType.Schema:
					result = srv.Databases[databaseName].Schemas[objectName];
					break;
				case SqlObjectType.FTCatalog:
					result = srv.Databases[databaseName].FullTextCatalogs[objectName];
					break;
			}
			return result;
		}


		/// <summary>
		/// Creates new database and restored data objects
		/// </summary>
		/// <param name="serverName">Server name</param>
		/// <param name="databaseName">Name of the database</param>
		/// <param name="inputDirName">Name of the input directory - this one points to the root dir for database scripts</param>
		/// <param name="dependencyFileName">Name of the dependency file for database objects</param>
		/// <param name="lockDatabase">Weather to put database to single user mode during operation</param>
		/// <param name="dataFilesDir">Where to put database data files (can be empty which means server default dir)</param>
		/// <param name="logger">Callback to pass log messages to caller</param>
		/// <returns></returns>
		public static void CreateNewDB(string serverName, string databaseName,
		                               string inputDirName,
		                               string dependencyFileName,
		                               bool lockDatabase,
		                               string dataFilesDir,
		                               TaskLoggingHelper logger)
		{

			if (inputDirName == string.Empty)
			{
				throw new InvalidArgumentException("You should provide input directory name");
			}

			Server srv = ConnectToServer(serverName);

			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgCreateNewDBInvocation,
			                                serverName, databaseName, inputDirName, dependencyFileName, lockDatabase));

			// check if database already exists
			bool databaseExists = srv.Databases.Contains(databaseName);

			if (!databaseExists && dataFilesDir == string.Empty)
			{
				dataFilesDir = (string)srv.ConnectionContext.ExecuteScalar(Resources.sqlGetMasterPath);
			}

			try
			{

				#region Find and execute all scripts for initial database setup
				// (except the one marked as 'create' in the case when database already exists)
				string[] createScriptList = Directory.GetFiles(inputDirName, "*.sql");
				foreach (string fileName in createScriptList)
				{
					if (!(fileName.Contains("create") && databaseExists))
					{
						logger.LogMessage("executing " + fileName);
						srv.ConnectionContext.ExecuteNonQuery(string.Format(File.ReadAllText(fileName), databaseName, dataFilesDir));
						srv.ConnectionContext.CommitTransaction();
					}
				}
				#endregion

				srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlUseDatabase, databaseName));

				if (lockDatabase)
				{
					srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlSingleUser, databaseName));
				}

				#region Restoring objects from files

				//if (inputDirName != string.Empty)
				//{
				//   CreateDBObjects(srv, inputDirName + @"\objects", dependencyFileName, logger);
				//}

				#endregion
			}
			finally
			{
				if (lockDatabase)
				{
					srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlMultiUser, databaseName));
				}
			}

		}


		/// <summary>
		/// Scripts data in tables defined by given tableMask and stores generated scripts
		/// to outputDir together with dependency file.
		/// </summary>
		/// <param name="serverName">Name of the server to connect to</param>
		/// <param name="databaseName">Name of the database to use</param>
		/// <param name="outputDir">Directory to save scripts to</param>
		/// <param name="dependencyFileName">Dependency file name</param>
		/// <param name="tableMask">Mask(s) of tables to </param>
		/// <param name="saveOptions"></param>
		/// <param name="logger"></param>
		public static void ScriptDBData(string serverName, string databaseName,
		                                string outputDir,
		                                string dependencyFileName,
		                                string tableMask,
		                                DataSaveOptions saveOptions,
		                                TaskLoggingHelper logger)
		{
			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgScriptDBDataInvocation,
			                                serverName, databaseName, outputDir, dependencyFileName, tableMask));

			Server srv = ConnectToServer(serverName, databaseName);

			DataTable _dependencyData = InitializeDependencyTable();

			string[] tableNameTemplates = tableMask.Split(';');

			foreach (string mask in tableNameTemplates)
			{
				// Get list of user tables matching supplied condition
				string sql = string.Format(Resources.sqlUserTablesMatchingTemplate, mask);
				DataTable tableList = srv.ConnectionContext.ExecuteWithResults(sql).Tables[0];

				foreach (DataRow r in tableList.Rows)
				{
					string tableName = (string)r["TableName"];
					string sortColumn = r.IsNull("idpresent") ? "" : (string)r["idpresent"];
					string identityColumn = r.IsNull("identitycolumn") ? "" : (string)r["identitycolumn"];
					string fullFileName = string.Format(@"{0}\{1}.data.sql", outputDir, tableName);
					ScriptTableData(srv.ConnectionContext.SqlConnectionObject, tableName, fullFileName, saveOptions, identityColumn, sortColumn);
					_dependencyData.Rows.Add(tableName, 0, Path.GetFileName(fullFileName));
				}
			}

			UpdateDependencies(SqlObjectType.UserTable, _dependencyData, srv);

			_dependencyData.WriteXml(outputDir + @"\" + dependencyFileName);

		}

		/// <summary>
		/// Fills given database with some (initial) data.
		/// </summary>
		/// <param name="serverName">Name of the server to connect to</param>
		/// <param name="databaseName">Name of the database to use</param>
		/// <param name="inputDir">Base directory for data insertion scripts</param>
		/// <param name="dependencyFileName"></param>
		/// <param name="logger"></param>
		public static void RestoreDBData(string serverName, string databaseName,
		                                 string inputDir,
		                                 string dependencyFileName,
		                                 TaskLoggingHelper logger)
		{
			Server srv = ConnectToServer(serverName, databaseName);

			CreateDBObjects(srv, inputDir, dependencyFileName, logger);
		}

		public static void RestoreDBObjects(string serverName, string databaseName,
		                                    string inputDirName,
		                                    string dependencyFileName,
		                                    bool lockDatabase,
		                                    bool dropExistingObjectcOfTheSameType,
		                                    TaskLoggingHelper logger)
		{

			if (inputDirName == string.Empty)
			{
				throw new InvalidArgumentException("You should provide input directory name");
			}

			Server srv = ConnectToServer(serverName, databaseName);

			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgRestoreDBObjectsInvocation,
			                                serverName, databaseName, inputDirName, dependencyFileName, lockDatabase, dropExistingObjectcOfTheSameType));

			try
			{

				if (lockDatabase)
				{
					srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlSingleUser, databaseName));
				}

				if (dropExistingObjectcOfTheSameType)
				{
					using (DataTable dependency = ReadDependencyFile(inputDirName, dependencyFileName, logger))
					{
						if (dependency == null)
						{
							return;
						}
						SqlObjectType objectTypesToDrop = GatherObjectTypes(dependency);
						DropDBObjects(srv, databaseName, objectTypesToDrop, logger);
					}
				}

				#region Restoring objects from files

				if (inputDirName != string.Empty)
				{
					CreateDBObjects(srv, inputDirName, dependencyFileName, logger);
				}

				#endregion
			}
			finally
			{
				if (lockDatabase)
				{
					srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlMultiUser, databaseName));
				}
			}

		}


		#endregion


		#region Private members

		private static void PrepareOutputDir(string outputDir, SqlObjectType objectTypes, ObjectSaveOptions saveOptions)
		{
			Directory.CreateDirectory(outputDir);

			foreach (object enumElement in Enum.GetValues(typeof(SqlObjectType)))
			{
				if ((objectTypes & (SqlObjectType)enumElement) != 0 
				    && (saveOptions & ObjectSaveOptions.SaveEachObjectTypeToSeparateDir)  != 0)
				{
					FieldInfo fi = typeof(SqlObjectType).GetField(enumElement.ToString());
					foreach (SavePropertiesAttribute saveProps in fi.GetCustomAttributes(typeof(SavePropertiesAttribute), false))
					{
						Directory.CreateDirectory(outputDir+"\\"+saveProps.DirName);
						foreach (string fileName in Directory.GetFiles(outputDir + "\\" + saveProps.DirName, "*.sql", SearchOption.TopDirectoryOnly))
						{
							File.Delete(fileName);
						}
					}
				}
			}
		}

		private static void ScriptTableData(SqlConnection con, string tableName, string fileName,
		                                    DataSaveOptions saveOptions, string identityColumnName, string orderByColumn)
		{
			StringBuilder result = new StringBuilder();
			int numRows;

			if (identityColumnName != string.Empty)
			{
				result.AppendFormat(Resources.sqlSetIdentityInsert, tableName, "ON");
				result.AppendLine();
			}

			using (SqlCommand com = new SqlCommand(string.Format(Resources.sqlGetTableData, tableName, orderByColumn), con))
			{
				DataTable data = new DataTable();
				data.Load(com.ExecuteReader());
				numRows = data.Rows.Count;

				string insertStatementTemplate = ComposeInsertStatementTemplate(tableName, data.Columns);

				foreach (DataRow r in data.Rows)
				{
					result.AppendFormat(insertStatementTemplate, RenderValues(r));
					result.AppendLine();
				}
			}

			if (identityColumnName != string.Empty)
			{
				result.AppendFormat(Resources.sqlSetIdentityInsert, tableName, "OFF");
				result.AppendLine();
			}

			File.WriteAllText(fileName, result.ToString(), Encoding.UTF8);

			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgTableData, tableName, numRows));

		}

		/// <summary>
		/// Prepares field values to be saved to SQL script - 
		/// quotes quotation marks and replaces null values with NULL
		/// </summary>
		/// <param name="r">DataRow to render</param>
		/// <returns></returns>
		private static object[] RenderValues(DataRow r)
		{
			object[] res = new object[r.ItemArray.Length];
			for (int i = 0; i < r.ItemArray.Length; i++)
			{
				string resAsString = r[i] as string;
				if (resAsString != null)
				{
					resAsString = resAsString.Replace("'", "''");
					res[i] = "'" + resAsString + "'";
				}
				else
				{
					res[i] = r.IsNull(i) ? "NULL" : r[i];
				}
			}
			return res;
		}

		/// <summary>
		/// Composes a template for inserting a row in a given table. This template can be later used
		/// with String.Format() to create INSERT statement for particular row.
		/// </summary>
		/// <param name="tableName">Name of the table</param>
		/// <param name="columns">Columns</param>
		/// <returns>Template for generating inserts for given table</returns>
		/// <remarks>Currently we need some special handling only for DateTime columns</remarks>
		private static string ComposeInsertStatementTemplate(string tableName, DataColumnCollection columns)
		{
			StringBuilder columnsList = new StringBuilder();
			StringBuilder valuesTemplate = new StringBuilder();
			for (int i = 0; i < columns.Count; i++)
			{
				columnsList.AppendFormat("[{0}]",columns[i].ColumnName);

				switch (columns[i].DataType.Name)
				{
					case "DateTime":
						valuesTemplate.Append("{" + i + ":s}");
						break;
					default:
						valuesTemplate.Append("{" + i + "}");
						break;
				}

				if (i != columns.Count - 1)
				{
					columnsList.Append(", ");
					valuesTemplate.Append(", ");
				}
			}

			return string.Format(Resources.sqlInsertTemplate, tableName, columnsList.ToString(), valuesTemplate.ToString());
		}


		/// <summary>
		/// Update dependency information
		/// </summary>
		/// <param name="objectTypes">Types of objects to query</param>
		/// <param name="objectsList">Dependency table containing objects of interest</param>
		/// <param name="srv">Server</param>
		private static void UpdateDependencies(SqlObjectType objectTypes, DataTable objectsList, Server srv)
		{
			DataTable depData = GetDependencies(objectTypes, srv);
			foreach (DataRow r in depData.Rows)
			{
				DataRow rowToUpdate = objectsList.Rows.Find(r["oObjName"]);
				if (rowToUpdate != null)
				{
					rowToUpdate["oRank"] = r["oSequence"];
				}
			}
		}

		private static DataTable GetDependencies(SqlObjectType objectTypes, Server srv)
		{
			string sql = Resources.sqlMSDependencyTemplate + Enum.Format(typeof(SqlObjectType), objectTypes, "x");
			DataTable depData = srv.ConnectionContext.ExecuteWithResults(sql).Tables[0];
			return depData;
		}

		/// <summary>
		/// Connects to SQL Server using SMO and sets connection properties
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="databaseName"></param>
		/// <returns></returns>
		private static Server ConnectToServer(string serverName, string databaseName)
		{
			ServerConnection srvCon;
			Server srv;

			srvCon = new ServerConnection();
			srvCon.ServerInstance = serverName;
			srvCon.DatabaseName = databaseName;
			srvCon.LoginSecure = true;
			srvCon.StatementTimeout = 300;
			srvCon.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
			srvCon.NonPooledConnection = true;

			srv = new Server(srvCon);
			return srv;
		}

		private static Server ConnectToServer(string serverName)
		{
			return ConnectToServer(serverName, "master");
		}

		private static int GetRequiredServerVersion(SqlObjectType objectType)
		{
			FieldInfo fi = typeof(SqlObjectType).GetField(objectType.ToString());
			object[] tmp = fi.GetCustomAttributes(typeof(RequiredServerVersion), false);
			return tmp.Length > 0 ? ((RequiredServerVersion)tmp[0]).Major : 0;
		}

		/// <summary>
		/// Compose file name depending on settings and object type
		/// </summary>
		/// <param name="objectName">Name of the object</param>
		/// <param name="objectType">Type of the object</param>
		/// <param name="saveOptions"></param>
		/// <returns>File name</returns>
		private static string GetFileName(string objectName, SqlObjectType objectType, ObjectSaveOptions saveOptions)
		{
			string result = objectName;
			string prefix = "";
			string suffix = "";
			string dirName = "";

			FieldInfo fi = typeof(SqlObjectType).GetField(objectType.ToString());
			foreach (SavePropertiesAttribute saveProps in fi.GetCustomAttributes(typeof(SavePropertiesAttribute), false))
			{
				prefix = saveProps.Prefix + ".";
				suffix = "." + saveProps.Suffix;
				dirName = saveProps.DirName;
			}

			if ((saveOptions & ObjectSaveOptions.AddObjectTypePrefix) > 0 && prefix != ".")
			{
				result = prefix + result;
			}
			if ((saveOptions & ObjectSaveOptions.AddObjectTypeSuffix) > 0)
			{
				result = result + suffix;
			}
			result = result + ".sql";
			if ((saveOptions & ObjectSaveOptions.SaveEachObjectTypeToSeparateDir) > 0)
			{
				result = dirName + "\\" + result;
			}

			return result;
		}

		/// <summary>
		/// Creates data table for storing dependency information
		/// </summary>
		/// <returns>Empty data table</returns>
		private static DataTable InitializeDependencyTable()
		{
			DataTable _dependencies;
			_dependencies = new DataTable("DependencyData");
			_dependencies.Columns.Add("oName", typeof(string));
			_dependencies.Columns.Add("oRank", typeof(int));
			_dependencies.Columns.Add("oFileName", typeof(string));
			_dependencies.Columns.Add("oType", typeof(string));
			_dependencies.Constraints.Add("PK_Dependencies", _dependencies.Columns["oName"], true);
			return _dependencies;
		}

		/// <summary>
		/// Convert binary array to hex string
		/// </summary>
		/// <param name="value"></param>
		/// <returns>Hexademical string</returns>
		/// <remarks>Currently used for getting human-readable represendation of md5 hash</remarks>
		private static string GetHexString(byte[] value)
		{
			StringBuilder res = new StringBuilder();
			foreach (byte val in value)
			{
				res.AppendFormat("{0:X}", val);
			}
			return res.ToString();
		}

		/// <summary>
		/// Save results of conversion script execution to sysConversionLog table in target database.
		/// </summary>
		/// <param name="srv">Connection to server</param>
		/// <param name="md5Hash">Algorithm for computing checksum of the script</param>
		/// <param name="fileName"></param>
		/// <param name="fileContent"></param>
		/// <param name="isSuccessful"></param>
		/// <param name="logger">Callback to report error back to caller</param>
		private static void SaveConversionResults(Server srv, string md5Hash,
		                                          string fileName, string fileContent, bool isSuccessful,
		                                          TaskLoggingHelper logger)
		{
			try
			{
				srv.ConnectionContext.ExecuteNonQuery(string.Format(Resources.sqlConversionLogData,
				                                                    Environment.MachineName, WindowsIdentity.GetCurrent().Name,
				                                                    DateTime.UtcNow.ToString("s"), isSuccessful ? "1" : "0",
				                                                    Path.GetFileName(fileName), fileContent.Replace("'", "''"), md5Hash));
			}
			catch (ExecutionFailureException ex)
			{
				logger.LogWarning(string.Format(Resources.msgConversionLogInsertFailure, ex.Message));
			}
		}

		/// <summary>
		/// Create a script for given object and store some output to provided dependency table
		/// </summary>
		/// <param name="outputDir">Base output directory</param>
		/// <param name="saveOptions">Options to be used for saving</param>
		/// <param name="dependenciesTable"></param>
		/// <param name="dropOptions">scripting options for DROP operation</param>
		/// <param name="createOptions">scripting options for CREATE operation</param>
		/// <param name="objectName">Name of the object to script</param>
		/// <param name="obj">The object itself</param>
		/// <param name="objectType">Type of the object</param>
		/// <param name="logger">Callback for logging</param>
		private static void ScriptObject(string outputDir, ObjectSaveOptions saveOptions,
		                                 DataTable dependenciesTable,
		                                 ScriptingOptions dropOptions, ScriptingOptions createOptions,
		                                 string objectName, IScriptable obj, SqlObjectType objectType,
		                                 TaskLoggingHelper logger)
		{
			string fileName = GetFileName(objectName, objectType, saveOptions);
			string fullFileName = outputDir + "\\" + fileName;
			Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
			dropOptions.FileName = fullFileName;
			createOptions.FileName = fullFileName;

			try
			{
				obj.Script(dropOptions);
				obj.Script(createOptions);

				dependenciesTable.Rows.Add(objectName, 0, fileName, objectType);

				Trace.WriteLineIf(DBTask.traceSwitch.TraceVerbose,
				                  string.Format(Resources.traceMsgScriptObject, objectName, objectType.ToString()));
			}
			catch (Exception e)
			{
				// Basically ignore errors
				logger.LogWarning(string.Format(Resources.msgScriptObjectFailure, objectName, objectType.ToString()));
				Trace.WriteLineIf(DBTask.traceSwitch.TraceWarning, e.ToString());
			}
		}


		/// <summary>
		/// Create database objects using provided dependency file
		/// </summary>
		/// <param name="srv">Server connection</param>
		/// <param name="inputDirName">Base directory</param>
		/// <param name="dependencyFileName">Dependency file name</param>
		/// <param name="logger">Logger callback</param>
		private static void CreateDBObjects(Server srv, 
		                                    string inputDirName, string dependencyFileName, TaskLoggingHelper logger)
		{
			Trace.WriteLineIf(DBTask.traceSwitch.TraceInfo, 
			                  string.Format(Resources.traceMsgCreateDBObjectsInvocation, inputDirName, dependencyFileName));

			// InitializeDependencyTable();
			// ToDo: add more fields to support more object types
			DataTable _dependencies = ReadDependencyFile(inputDirName, dependencyFileName, logger);
			if (_dependencies == null)
			{
				return;
			}

			try
			{
				_dependencies.DefaultView.Sort = "oRank ASC";
				for (int i = 0; i < _dependencies.DefaultView.Count; i++)
				{
					string fileName = _dependencies.DefaultView[i]["oFileName"].ToString();
					try
					{
						srv.ConnectionContext.ExecuteNonQuery(File.ReadAllText(inputDirName + "\\" + fileName));
						Trace.WriteLineIf(DBTask.traceSwitch.TraceVerbose,
						                  string.Format(Resources.traceMsgRestoreObject, fileName));
					}
					catch (IOException ex)
					{
						logger.LogError(string.Format(Resources.msgFailure, fileName, ex.Message));
						Trace.WriteLineIf(DBTask.traceSwitch.TraceError, ex.ToString());
					}
					catch (ExecutionFailureException ex)
					{
						logger.LogError(string.Format(Resources.msgFailure, fileName, ex.Message));
						Trace.WriteLineIf(DBTask.traceSwitch.TraceError, ex.ToString());
					}
				}
				logger.LogMessage(string.Format("Processed {0} objects for database {1}", _dependencies.Rows.Count, srv.ConnectionContext.DatabaseName));
			}
			catch (ConstraintException ex)
			{
				StringBuilder exceptionData = new StringBuilder();
				exceptionData.AppendLine(Resources.msgDependencyFileConstraintViolation);
				foreach (DictionaryEntry de in ex.Data)
				{
					exceptionData.AppendFormat("{0} = {1}", de.Key, de.Value);
				}
				logger.LogError(exceptionData.ToString());
			}
		}

		#endregion

	}
}