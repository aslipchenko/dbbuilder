# Problem statement #
In the current state of development process, when many people are involved in version development and deployment, application database change management, version update and maintenance becomes more and more difficult. At the moment, database development process is poorly documented and hardly manageable. We need to define such a process that will simplify DB development and upgrade from version to version.

# Our Goals #
In order to simplify development and deployment process we need:
Develop a standardized way of making changes to DB
Minimize time spent on synchronizing and updating different instances of database
Let the process of DB update be performed in automated manner (as a part of build/deployment process)
Below you will find description of our DB development and deployment process and how it addresses each of the issues listed above.

# Development process #
For the sake of simplicity we divide all database objects into two major categories:
  * data - tables (including table constraints and indexes), views (Note: for SPS database we have only database tables belonging to data objects (as views depend on UDFs sometimes))
  * logic - other non-data objects like stored procedures, triggers, user-defined function, etc.
Below we will use {Database} to denote database name being a part of a path.


Data objects are changed by conversion scripts and should be to SVN under (for Admin database) trunk/source/database/{Database}/conversion. You can find some guidelines for writing scripts below:
  * script name should include sequence number and purpose denoted in file name (e.g.: 0037\_alter\_someTable.sql, 0056\_data\_someOtherTable.sql, etc. By introductin naming convention for conversion scripts we persue 2 main goals:
    1. ensure porper (chronological) order of scripts
    1. make it easier to understand why the script is needed for (so we don't need to look inside in most cases)
  * Each script should contain comments, stating when and who created the script and for what purpose (this could be reference to bug/issue number or something) similar to this:
```
/*******************************************************************************/
/* developer 2008/11/07 Created                                                */
/* Description of what this script actually does                               */
/*******************************************************************************/
```
  * Each script should be wrapped in transaction. It is up to developer to chose correct isolation level however in most cases default value (READ COMMITTED) should do the job. It is better to use named transaction. You can use the same name as in conversion script (alter\_someTable and data\_someOtherTable for the above mentioned examples). In order to ensure proper transaction rollback you should set XACT\_ABORT to ON before transaction start.
  * Though we are going have a mechanism ensuring that each script will be executed only once for each database, developer is responsible for writing script in such a way that its' subsequent executions don't lead to error or incorrect database state. For example, if you need to create a table you should first check if it already exists.
  * Data conversion scripts are considerer strictly sequential, i.e. they are not supposed to be excluded from build. This is why they should be committed separately from code. This is a limitation of cause, but it grants us more   control on data conversion process. You can use following simple rules to make your scripts more bullet-proof:
    * Try to extend schema and not to narrow it. You can always drop unused column (when it is absolutely not used any more). The same with tables. In the case you introduce new table for your code, but at some point of time it is decided that the code is not ready - the table (which is anyway included) is already in database, but it is not used. This is a minor problem which can (and will) be easily solved.
    * In the case you mention typo or error (meaning logical and not syntax error) in your conversion script you should not modify original script but better write a new one to make things correct. The same is with changes you absolutely need to rollback - you just create a new script.

You can find some useful examples here: Conversion script best practices.

Logic is changed in a different way. For each object of this type we maintain a separate script in SVN under trunk/source/database/{Database}/objects/{Object type}. Object types are: StoredProcedure, Trigger, UDF. In the case you need to modify and object, you should find corresponding file, modify it and commit to SVN together with the code that requires new version.
Please, follow DB objects naming conventions when creating new objects in any of our databases. Also note that conversion scripts as well as database objects are subject to review. It is ideal to discuss the changes you want to introduce with a person responsible for database development.

# Deployment/upgrade process #
Database upgrade is an integral part of deployment process. We have a custom MSBuild task for performing this action. Database upgrade process consists of the following steps:
  1. Get required all required files (from the commits you need). This is done during application build. We are mainly interested in conversion scripts and files defining database logic. In terms of SVN we need trunk/source/database/{Database}
  1. Put database to single user mode effectively preventing access to it. During upgrade process we con not afford other connections as side activity can lead to
  1. Drop all logic from the database (i.e. all stored procedures, user-defined functions, etc.)
  1. Apply conversion scripts
    * Scripts are applied in order defined by sequence number
    * In target database we have a special table sysConversionLog which holds a whole history of conversion scripts applied to this database. This table is not user-modifiable and you should not worry about filling/querying this table in conversion scrips - all will be done automatically (by MSBuild task)
    * We use MD5 checksum to determine if given script was already executed for target database. In the case it was, we simply ignore it and move to the next one. We ignore the script only in the case when it's execution was successful, so in the case we executed the script before, but it raised an error, we will try to execute it once more.
    * Result of script execution together with script body is stored in sysConversionLog.
  1. Recreate database logic from scripts
    * We have a special file that describes interdependencies between database objects and allows us to create them in correct sequence. This file can be found under trunk/source/database/{Database}/objects/ and is named logicobjects.dependencies.xml
  1. Database is put back to multi-user mode, upgrade complete