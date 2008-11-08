using System;
using System.Collections.Generic;
using System.Text;

namespace DBBuilder.MSSQL
{
	/// <summary>
	/// Lets specification of target SqlServer version for given object type.
	/// Object is processed in the case targer server version (major) is higher that given
	/// </summary>
	public class RequiredServerVersion : Attribute
	{
		public RequiredServerVersion(int major)
		{
			this._major = major;
		}

		private int _major;
		public int Major 
		{
			get { return _major; }
			set { _major = value; }
		}
	}

	public class SavePropertiesAttribute : Attribute
	{
		public SavePropertiesAttribute(string prefix, string suffix, string dirName)
		{
			this._prefix = prefix;
			this._suffix = suffix;
			this._dirName = dirName;
		}

		private string _prefix;
		public string Prefix
		{
			get { return _prefix; }
			set { _prefix = value; }
		}

		private string _suffix;
		public string Suffix
		{
			get { return _suffix; }
			set { _suffix = value; }
		}

		private string _dirName;
		public string DirName
		{
			get { return _dirName; }
			set { _dirName = value; }
		}	
	}

	/// <summary>
	/// SQL object types
	/// </summary>
	[Flags()]
	public enum SqlObjectType
	{
		None = 0x0000,

		[SaveProperties("udf", "udf", "UDF")]
		UserDefinedFunction = 0x0001,

		SystemObject = 0x0002,

		[SaveProperties("vw", "view", "View")]
		View = 0x0004,

		[SaveProperties("tbl", "table", "Table")]
		UserTable = 0x0008,
		
		[SaveProperties("sp", "sp", "StoredProcedure")]
		StoredProcedure = 0x0010,

		[SaveProperties("tr", "trigger", "Trigger")]
		[RequiredServerVersion(9)]
		Trigger = 0x0100,

		[SaveProperties("ass", "assembly", "Assembly")]
		[RequiredServerVersion(9)]
		Assembly = 0x0200,

		[SaveProperties("udt", "udt", "UDT")]
		UserDefinedType = 0x0400,

		[SaveProperties("", "schema", "Schema")]
		[RequiredServerVersion(9)]
		Schema = 0x0800,

		[SaveProperties("", "ftcatalog", "FTCatalog")]
		FTCatalog = 0x1000

		// Constants for uniting object types
		//logicTypes = SqlObjectType.StoredProcedure | SqlObjectType.Trigger | SqlObjectType.UserDefinedFunction | SqlObjectType.Assembly,
		//dataTypes = SqlObjectType.UserTable | SqlObjectType.View,
		//securityTypes = SqlObjectType.Schema

	}

	/// <summary>
	/// Options available for saving scripts of database objects
	/// </summary>
	[Flags()]
	public enum ObjectSaveOptions
	{
		AddObjectTypeSuffix = 1,
		AddObjectTypePrefix = 2,
		SaveEachObjectTypeToSeparateDir = 4,
	}

	/// <summary>
	/// Options available for saving table data
	/// </summary>
	[Flags()]
	public enum DataSaveOptions
	{
		None = 0x0000,

		PreserveIDs = 0x0001
	}
}