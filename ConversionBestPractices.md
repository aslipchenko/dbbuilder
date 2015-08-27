Below you will find samples of common conversion tasks. They show proposed practice for implementing conversion scripts.

  * Creating a table
  * Adding a column
  * Adding index
  * Recreate table


# Creating a table #
```

/*******************************************************************************/
/* megadeveloper 2008/03/08 Created                                            */
/* Create table SomeTable to fulfil some important requirement                 */
/*******************************************************************************/
SET XACT_ABORT ON
BEGIN TRANSACTION create_SomeTable

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'SomeTable' AND xtype = 'U') GOTO lbl_Exit


CREATE TABLE SomeTable
(
	...
);

lbl_Exit:
COMMIT TRANSACTION create_SomeTable
```


# Adding a column #

```
/*******************************************************************************/
/* megadeveloper 2008/04/01 Created                                            */
/* Add column ColumnToAdd to table SomeTable (Mantis 65535)                    */
/*******************************************************************************/
SET XACT_ABORT ON
BEGIN TRANSACTION alter_SomeTable

IF EXISTS (SELECT * FROM syscolumns WHERE name = 'ColumnToAdd' AND ID = OBJECT_ID('SomeTable')) GOTO lbl_Exit


-- Simplest case: allow NULLs, no default value 
ALTER TABLE SomeTable ADD ColumnToAdd int NULL;

-- Not nullable, with default value, update existing rows, specify constraint name

ALTER TABLE SomeTable ADD ColumnToAdd int NULL;
ALTER TABLE SomeTable ADD CONSTRAINT [DF_SomeTable_ColumnToAdd] DEFAULT -1 FOR ColumnToAdd WITH VALUES;
ALTER TABLE SomeTable ALTER COLUMN ColumnToAdd int NOT NULL;


lbl_Exit:
COMMIT TRANSACTION alter_SomeTable

```

# Adding index #

```
/*******************************************************************************/
/* megadeveloper 2008/01/01 Created                                            */
/* Add new index IndexToAdd to table SomeTable in order to improve something   */
/*******************************************************************************/
SET XACT_ABORT ON
BEGIN TRANSACTION alter_SomeTable

IF EXISTS (SELECT * FROM sysindexes WHERE name = 'IndexToAdd' AND ID = OBJECT_ID('SomeTable')) GOTO lbl_Exit


CREATE INDEX IndexToAdd ON SomeTable (<column list>) WITH ( FILLFACTOR = 90 );

lbl_Exit:
COMMIT TRANSACTION alter_SomeTable

```

# Recreate table #

```
/*******************************************************************************/
/* megadeveloper 2008/05/09 Created                                            */
/* Recreate table SomeTable preserving data                                    */
/*******************************************************************************/
SET XACT_ABORT ON
BEGIN TRANSACTION recreate_SomeTable

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'z_SomeTable') GOTO lbl_Exit


-- Step 1: Drop all indexes/keys/constraints from original table
...

-- Step 2: Rename outdated table
EXEC sp_rename 'SomeTable', 'z_SomeTable', 'OBJECT'


-- Step 3: create new table
CREATE TABLE SomeTable ( ... );

-- Step 4: copy data
INSERT INTO SomeTable ( ... )
SELECT ...
FROM z_SomeTable;



lbl_Exit:
COMMIT TRANSACTION recreate_SomeTable
```