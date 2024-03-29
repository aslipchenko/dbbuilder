﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBBuilder.MSSQL {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DBBuilder.MSSQL.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to save conversion script execution log to sysConversionLog: {0} (most likely that the table is missing)..
        /// </summary>
        internal static string msgConversionLogInsertFailure {
            get {
                return ResourceManager.GetString("msgConversionLogInsertFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleted all {0} from {1}.
        /// </summary>
        internal static string msgDeletedObjects {
            get {
                return ResourceManager.GetString("msgDeletedObjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Constrain violation exception raised during dependency file load. It is likely that dependency file contains duplication entries. Additional exception data follows..
        /// </summary>
        internal static string msgDependencyFileConstraintViolation {
            get {
                return ResourceManager.GetString("msgDependencyFileConstraintViolation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed - {0}, {1}.
        /// </summary>
        internal static string msgFailure {
            get {
                return ResourceManager.GetString("msgFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Finished conversion of database {0}, processed {1} files..
        /// </summary>
        internal static string msgFinishedConversion {
            get {
                return ResourceManager.GetString("msgFinishedConversion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You should provide valid (non-empty) dependency file name..
        /// </summary>
        internal static string msgNoDependencyFileName {
            get {
                return ResourceManager.GetString("msgNoDependencyFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OK - {0}.
        /// </summary>
        internal static string msgOK {
            get {
                return ResourceManager.GetString("msgOK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to script object  &apos;{0}&apos; of type {1}.
        /// </summary>
        internal static string msgScriptObjectFailure {
            get {
                return ResourceManager.GetString("msgScriptObjectFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Script file {0} was executed already on {2} GMT (sysConversionLog.ID = {1}).
        /// </summary>
        internal static string msgScriptWasExecutedBefore {
            get {
                return ResourceManager.GetString("msgScriptWasExecutedBefore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All subsequent conversion scripts will be skipped..
        /// </summary>
        internal static string msgSkipConversionScriptsAfterError {
            get {
                return ResourceManager.GetString("msgSkipConversionScriptsAfterError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO sysConversionLog (HostName, AccountName,gmtActionDate,IsSuccessful, FileName,FileBody,CheckSum)
        ///VALUES (&apos;{0}&apos;, &apos;{1}&apos;,&apos;{2}&apos;,{3},&apos;{4}&apos;,&apos;{5}&apos;,&apos;{6}&apos;);.
        /// </summary>
        internal static string sqlConversionLogData {
            get {
                return ResourceManager.GetString("sqlConversionLogData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ID, gmtActionDate FROM sysConversionLog WHERE CheckSum = &apos;{0}&apos; AND IsSuccessful = 1.
        /// </summary>
        internal static string sqlFindConversionFileByCheckSum {
            get {
                return ResourceManager.GetString("sqlFindConversionFileByCheckSum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select top 1 
        ///	substring(rtrim(filename), 1, len(rtrim(filename)) - charindex(&apos;\&apos;, reverse(rtrim(filename)))+1)
        ///from master.dbo.sysfiles order by fileid;.
        /// </summary>
        internal static string sqlGetMasterPath {
            get {
                return ResourceManager.GetString("sqlGetMasterPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM {0}.
        /// </summary>
        internal static string sqlGetTableData {
            get {
                return ResourceManager.GetString("sqlGetTableData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO {0} ({1}) VALUES ({2});.
        /// </summary>
        internal static string sqlInsertTemplate {
            get {
                return ResourceManager.GetString("sqlInsertTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to exec sp_MSdependencies @flags = 0x.
        /// </summary>
        internal static string sqlMSDependencyTemplate {
            get {
                return ResourceManager.GetString("sqlMSDependencyTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER DATABASE [{0}] SET MULTI_USER.
        /// </summary>
        internal static string sqlMultiUser {
            get {
                return ResourceManager.GetString("sqlMultiUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SET IDENTITY_INSERT [{0}] {1}.
        /// </summary>
        internal static string sqlSetIdentityInsert {
            get {
                return ResourceManager.GetString("sqlSetIdentityInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;.
        /// </summary>
        internal static string sqlSingleUser {
            get {
                return ResourceManager.GetString("sqlSingleUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to USE [{0}];.
        /// </summary>
        internal static string sqlUseDatabase {
            get {
                return ResourceManager.GetString("sqlUseDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select 
        ///	o.name as TableName, 
        ///	c_id.name as idpresent,
        ///	c_ident.name as identitycolumn
        ///from sysobjects o
        ///left join syscolumns c_id on o.id = c_id.id and c_id.name = &apos;id&apos;
        ///left join syscolumns c_ident on o.id = c_ident.id and c_ident.status &amp; 0x80 &gt; 0
        ///where o.name like &apos;{0}&apos; and o.xtype=&apos;U&apos;.
        /// </summary>
        internal static string sqlUserTablesMatchingTemplate {
            get {
                return ResourceManager.GetString("sqlUserTablesMatchingTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.CreateDBObjects invoked, dependency file name = {0}\{1}.
        /// </summary>
        internal static string traceMsgCreateDBObjectsInvocation {
            get {
                return ResourceManager.GetString("traceMsgCreateDBObjectsInvocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.CreateNewDB: server = {0}, database = {1}, inputDir = {2}, dependencyFile = {3}, lock database = {4}.
        /// </summary>
        internal static string traceMsgCreateNewDBInvocation {
            get {
                return ResourceManager.GetString("traceMsgCreateNewDBInvocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.DropDBObjects: Dropping following object types: {0}.
        /// </summary>
        internal static string traceMsgDropDBObjectsInvocation {
            get {
                return ResourceManager.GetString("traceMsgDropDBObjectsInvocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dropped {0}.
        /// </summary>
        internal static string traceMsgObjectDropped {
            get {
                return ResourceManager.GetString("traceMsgObjectDropped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.RestoreDBObjects: server = {0}, database = {1}, inputDir = {2}, dependencyFile = {3}, lock database = {4}, drop existing objects of the same type = {5}.
        /// </summary>
        internal static string traceMsgRestoreDBObjectsInvocation {
            get {
                return ResourceManager.GetString("traceMsgRestoreDBObjectsInvocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Successfully executed {0}.
        /// </summary>
        internal static string traceMsgRestoreObject {
            get {
                return ResourceManager.GetString("traceMsgRestoreObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.ScriptDBData: server = {0}, database = {1}, outputDir = {2}, dependencyFile = {3}, table mask = {4}.
        /// </summary>
        internal static string traceMsgScriptDBDataInvocation {
            get {
                return ResourceManager.GetString("traceMsgScriptDBDataInvocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.ScriptDB: database = {0}, object types = {3}, dependency file = {1}\{2}.
        /// </summary>
        internal static string traceMsgScriptDBInvocation {
            get {
                return ResourceManager.GetString("traceMsgScriptDBInvocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scripted {0} of type {1}.
        /// </summary>
        internal static string traceMsgScriptObject {
            get {
                return ResourceManager.GetString("traceMsgScriptObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Skipping {0} as current server version is lover that required ({1}).
        /// </summary>
        internal static string traceMsgSkippingObjectTypeDueToLowServerVersion {
            get {
                return ResourceManager.GetString("traceMsgSkippingObjectTypeDueToLowServerVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scripted {1} rows from {0}.
        /// </summary>
        internal static string traceMsgTableData {
            get {
                return ResourceManager.GetString("traceMsgTableData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SqlServerHelper.UpgradeDB: server = {0}, database = {1}, inputDir = {2}, dependencyFile = {3}, conversionDir = {4}, lock database = {5}.
        /// </summary>
        internal static string traceMsgUpgradeDBInvocation {
            get {
                return ResourceManager.GetString("traceMsgUpgradeDBInvocation", resourceCulture);
            }
        }
    }
}
