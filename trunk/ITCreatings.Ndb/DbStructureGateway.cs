using System;
using System.Collections.Generic;
using System.Reflection;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb
{
    /// <summary>
    /// Gateway to Database Structure. Contains SDL commands wrappers.
    /// </summary>
    public class DbStructureGateway
    {
        #region Singleton

        private static DbStructureGateway instance;

        /// <summary>
        /// DbStructureGateway to the DbAccessor.Instance
        /// </summary>
        public static DbStructureGateway Instance
        {
            get
            {
                //TODO: add multithread support?
                if (instance == null)
                    instance = new DbStructureGateway(DbAccessor.Instance);

                return instance;
            }
        }

        #endregion

        #region Constructor&Fields

        private readonly DbAccessor accessor;

        /// <summary>
        /// Accessor to underlayed DbAccessor
        /// </summary>
        public DbAccessor Accessor { get { return accessor; } }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="accessor"></param>
        public DbStructureGateway(DbAccessor accessor)
        {
            this.accessor = accessor;
        }

        #endregion

        #region Object Methods

        private Assembly assembly;

        /// <summary>
        /// This Assembly will be used by Gateway to search objects with DbRecordAttribute 
        /// </summary>
        public Assembly Assembly
        {
            get
            {
                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                return assembly;
            }
            set
            {
                assembly = value;
            }
        }

        /// <summary>
        /// Creates Tables Associated with Objects from <see cref="Assembly"/>
        /// </summary>
        public void CreateTables()
        {
            CreateTables(Assembly);
        }

        /// <summary>
        /// Removes Tables Associated with Objects From <see cref="Assembly"/>
        /// </summary>
        public void DropTables()
        {
            DropTables(Assembly);
        }

        #endregion

        #region Interface

        /// <summary>
        /// Contains Last Error
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// Checks if assosiated table exists, and all fields marked as DbFieldAttribute present in database
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsValid(Type type)
        {
            if (!IsTableExists(type))
                return false;

            DbTableCheckResult result = new DbTableCheckResult(accessor);
            bool isAllFieldValid = result.IsAllFieldValid(type);
            
            if (!isAllFieldValid)
                LastError = result.LastError;

            return isAllFieldValid;
        }

        /// <summary>
        /// Determines whether the specified types are valid.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>
        /// 	<c>true</c> if the specified types is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(Type [] types)
        {
            foreach (Type type in types)
            {
                if (!IsValid(type))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified assembly is valid.
        /// </summary>
        /// <param name="sourceAssembly">The assembly.</param>
        /// <returns>
        /// 	<c>true</c> if the specified assembly is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(Assembly sourceAssembly)
        {
            Type[] types = DbAttributesManager.LoadDbRecordTypes(sourceAssembly);
            return IsValid(types);
        }
        

        /// <summary>
        /// Checks is Associated Table Exists
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsTableExists(Type type)
        {
            return IsTableExists(DbAttributesManager.GetTableName(type));
        }

        /// <summary>
        /// Checks is specifyed table exists
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool IsTableExists(string tableName)
        {
            try
            {
                accessor.LoadCount(tableName);
                return true;
            }
            catch (NdbConnectionFailedException)
            {
                throw;
            }
            catch(Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Creates associated table
        /// </summary>
        /// <param name="type"></param>
        public void CreateTable(Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            accessor.CreateTable(info);
        }

        /// <summary>
        /// Creates associated table and all foreign keys tables
        /// </summary>
        /// <param name="type">Returns true if table created otherwise false</param>
        public bool CreateTableEx(Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            return CreateTableEx(info);
        }

        private bool CreateTableEx(DbRecordInfo info)
        {
            if (IsTableExists(info.TableName))
                return false;

            foreach (KeyValuePair<Type, DbFieldInfo> key in info.ForeignKeys)
            {
                if (key.Key != info.RecordType)
                {
                    DbIdentityRecordInfo primaryInfo = DbAttributesManager.GetRecordInfo(key.Key) as DbIdentityRecordInfo;
                    if (primaryInfo == null)
                        throw new NdbNotIdentityException(string.Format(
                            "Records without primary kes can't be used as foreign keys.\r\nRecord {0}.\r\nPrimary record type {1}"
                            , info.TableName, key.Key));

                    if (primaryInfo.PrimaryKey.FieldType != key.Value.FieldType)
                        throw new NdbException(
                            "Primary key {0} in {1} is {2}. But Foreign key {3} in {4} is {5}",
                            primaryInfo.PrimaryKey.Name, primaryInfo.TableName, primaryInfo.PrimaryKey.FieldType,
                            key.Value.Name, info.TableName, key.Value.FieldType);

                    CreateTableEx(primaryInfo);
                }
            }

            accessor.CreateTable(info);

            return true;
        }

        /// <summary>
        /// Removes Table from Database
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool DropTable(Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);

            return accessor.DropTableSafe(info.TableName);
        }


        /// <summary>
        /// Removes Tables Associated with Records From cpecifyed Assembly
        /// </summary>
        /// <param name="targetAssembly"></param>
        public Type[] DropTables(Assembly targetAssembly)
        {
            Type[] types = DbAttributesManager.LoadDbRecordTypes(targetAssembly);

            int i = 0;

            // so "pretty" code tryes to remove all tables. 
            // but checks if we have some problems like not connected to database
            while (true)
            {
                if (i++ == types.Length)
                {
                    throw new NdbException("Can't drop tables from the following assembly: " + 
                        targetAssembly.GetName());    
                }

                bool allSuccess = true;
                foreach (Type type in types)
                {
                    if (!DropTable(type))
                        allSuccess = false;
                }

                if (allSuccess)
                    break;
            }

            return types;
        }

        /// <summary>
        /// Create Tables for all classes with DbRecordAttribute from specifyed Assembly
        /// </summary>
        /// <param name="sourceAssembly"></param>
        /// <returns>Returns only types tables for which were created</returns>
        public Type[] CreateTables(Assembly sourceAssembly)
        {
            Type[] types = DbAttributesManager.LoadDbRecordTypes(sourceAssembly);
            return CreateTables(types);
        }

        /// <summary>
        /// Create tables for all specifyed types
        /// </summary>
        /// <param name="types"></param>
        /// <returns>Returns only types tables for which were created</returns>
        public Type[] CreateTables(Type[] types)
        {
            List<Type> list = new List<Type>(types.Length);
            foreach (Type type in types)
                if (CreateTableEx(type))
                    list.Add(type);

            return list.ToArray();
        }

        /// <summary>
        /// Updates database tables to match types from passed assembly
        /// </summary>
        /// <param name="sourceAssembly"></param>
        /// <returns>List of altered tables</returns>
        public Type[] AlterTables(Assembly sourceAssembly)
        {
            Type[] types = DbAttributesManager.LoadDbRecordTypes(sourceAssembly);
            return AlterTables(types);
        }

        /// <summary>
        /// Update database tables to match specifyed types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Type[] AlterTables(Type[] types)
        {
            foreach (Type type in types)
                AlterTableEx(type);

            return types;
        }

        ///<summary>
        /// Updates database table to match passed Type
        /// 
        /// Notice:
        ///  - affrects only added and changed fields
        ///  - doesn't remove unused columns from database
        ///  - ignores primary key and indexes
        ///</summary>
        ///<param name="type">Type to check</param>
        ///<returns>true on success</returns>
        public bool AlterTable(Type type)
        {
            DbTableCheckResult result = new DbTableCheckResult(accessor);
            result.Build(type);
            accessor.AlterTable(result);
            return true;
        }

        /// <summary>
        /// Update database table to match passed Type
        /// 
        /// Extended features:
        ///  - creates\updates foreign key tables
        ///  - create table if it doesn't exists
        /// </summary>
        /// <param name="type"></param>
        public void AlterTableEx(Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            AlterTableEx(info);
        }

        private void AlterTableEx(DbRecordInfo info)
        {
            foreach (KeyValuePair<Type, DbFieldInfo> key in info.ForeignKeys)
            {
                DbIdentityRecordInfo primaryInfo = DbAttributesManager.GetRecordInfo(key.Key) as DbIdentityRecordInfo;
                if (primaryInfo == null)
                    throw new NdbNotIdentityException(string.Format(
                                                          "Records without primary kes can't be used as foreign keys.\r\nRecord {0}.\r\nPrimary record type {1}"
                                                          , info.TableName, key.Key));

                if (primaryInfo.PrimaryKey.FieldType != key.Value.FieldType)
                    throw new NdbException(
                        "Primary key {0} in {1} is {2}. But Foreign key {3} in {4} is {5}",
                        primaryInfo.PrimaryKey.Name, primaryInfo.TableName, primaryInfo.PrimaryKey.FieldType,
                        key.Value.Name, info.TableName, key.Value.FieldType);

                AlterTableEx(primaryInfo);
            }

            if (IsTableExists(info.TableName))
            {
                if (!IsValid(info.RecordType))
                    AlterTable(info.RecordType);
            }
            else
                CreateTable(info.RecordType);
        }

        #endregion
    }
}