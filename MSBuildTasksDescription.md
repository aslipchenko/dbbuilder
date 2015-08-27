Below you will find description of msbuild custom tasks defined in DBBuilder.MSSQL assembly.

  * DBCreateNewTask
  * DBRestoreDataTask
  * DBScriptDataTask
  * DBScriptTask
  * DBUpgradeTask

# DBCreateNewTask #
Parameters:

  * ServerName (req): name of the database server
  * DatabaseName (req): name of the database to be created
  * SourceDirName (req):
  * DependencyFileName: name of dependency file to use

What it does: Creates new database instance with given name.
How it works:

  1. All sql scripts from SourceDirName are executed in order. In the case database with give name already exists we skip scripts that contain 'create' in their names.
  1. Database objects are restored using given dependency file (SourceDirName \ objects \ DependencyFileName).

# DBRestoreObjectsTask #
Parameters:

  * ServerName (req): name of the database server
  * DatabaseName (req): name of the target database
  * SourceDirName (req):
  * DependencyFileName (req): name of dependency file to use. If value for this property is not provided, a default value of ".dependencies.xml" is use.
  * DropExistingObjectsOfTheSameType: whether to drop all objects of the types found in dependency file prior to restoration (defaults to "true")

What it does: Resores database objects
How it works:

  1. Takes SourceDirName \ objects \ DependencyFileName and executes all references files in order of rank


# DBRestoreDataTask #
Parameters:

  * ServerName (req): name of the database server
  * DatabaseName (req): name of the target database
  * SourceDirName (req):
  * DependencyFileName: name of dependency file to use. If value for this property is not provided, a default value of ".dependencies.xml" is use.

What it does: Fills target database with data.
How it works:

  1. Takes SourceDirName \ DependencyFileName and executes all references files in order of rank


# DBScriptDataTask #
Parameters:

  * ServerName (req): name of the database server
  * DatabaseName (req): name of the target database
  * DestinationDir (req): name of the folder to store results to
  * DependencyFileName: name of dependency file to use. If value for this property is not provided, a default value of ".dependencies.xml" is use.
  * TableMask: mask(s) that specifies tables to script. This value is used in LIKE operator so it accepts corresponding wildcards (% for 'any symbol(s)', etc.). Default is '%', i.e. 'all tables'. You can specify more that one mask by separating them with semicolon, e.g. 'dic%;tblAccount'.

What it does: Scripts table data in form of INSERT statements.
How it works:

  * Table masks in TableMask are analyzed one by one, matching tables are scripted and files are saved to DestinationDir
  * After all tables are saved, dependency file is composed to store information about tables dependencies.



# DBScriptTask #
Parameters:

  * ServerName (req): name of the database server
  * DatabaseName (req): name of the target database
  * DestinationDir (req):
  * DependencyFileName: name of dependency file to use
  * ObjectsToScript: determines which objects to save. Possible values: Data (tables and views), Logic (stored procedures, user-defined functions, triggers) [is default value](this.md), All (Data + Logic).

What it does: Creates scripts of database objects.
How it works:

  * DestinationDir is cleared according to ObjectsToScript parameter. We have a separate directory for each object type ("StoredProcedure", "Table", "Trigger", "UDF", "View" respectively). During this phase all affected subdirectories are cleared, i.e. all sql files are deleted.
  * Objects are scripted and saved to appropriate directory. File name is composed according to pattern (..sql).
  * Dependency information is written to DestinationDir \ DependencyFileName



# DBUpgradeTask #
Parameters:

  * ServerName (req): name of the database server
  * DatabaseName (req): name of the target database
  * DependencyFileName: name of dependency file to use
  * ConversionDirName:
  * SourceDirName:


What it does: Upgrades target database
How it works: .

  1. Database is put to single-user mode, option WITH ROLLBACK IMMEDIATE is used to close existing connections
  1. All logic objects (stored procedures, user-defined functions, triggers, assemblies) are deleted
  1. Conversion scripts in ConversionDirName are read one by one. For each script MD5 chacksum is computed. Then sysConversionLog table in target database is analyzed. In the case when script with the same checksum was successfully executed, it is skipped. Upon execution results are saved to sysConversionLog. Execution results include host name, account name, execution time (UTC), result (success or fail), script checksum and script body. In the case there was an exception during script execution, all subsequent conversion scripts are skipped.
  1. Provided dependency file (SourceDirName \ DependencyFileName) is used to restore objects. It is supposed that this file contains references to Logic objects (i.e. those one, dropped on the first step) but there's no explisit or implicit check for this - all scripts listed in dependency file are executed in order. Exception during execution of these scripts doesn't lead to skipping remaining ones.
  1. Database is put back to multiuser mode.