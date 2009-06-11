using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Query;
using ITCreatings.Ndb.Utils;

namespace ITCreatings.Ndb
{
    /// <summary>
    /// Gateway to Database Data. The core class of the Ndb. Contains DDL commands wrappers
    /// </summary>
    public class DbGateway
    {
        #region Singleton

        private static DbGateway instance;
        ///<summary>
        /// Singleton accessor
        ///</summary>
        public static DbGateway Instance
        {
            get
            {
                //TODO: add multithread support?
                if (instance == null)
                    instance = new DbGateway(DbAccessor.Instance);

                return instance;
            }
        }

        #endregion

        #region Constructor & properties

        private readonly DbAccessor Da;

        /// <summary>
        /// Provides access to underlayed DbAccessor
        /// </summary>
        public DbAccessor Accessor { get { return Da; } }
        
        /// <summary>
        /// Creates new DbGateway instance
        /// </summary>
        /// <param name="accessor">DbAccessor</param>
        public DbGateway(DbAccessor accessor)
        {
            Da = accessor;
        }

        #endregion

        #region Interface

        #region DbQuery

        /// <summary>
        /// Creates select query object
        /// </summary>
        /// <returns></returns>
        public DbQuery Select()
        {
            return DbQuery.Create(this);
        }

        /// <summary>
        /// Creates select query object using specified expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        public DbQuery Select(List<DbFilterNode> expressions)
        {
            return DbQuery.Create(this, expressions);
        }

        /// <summary>
        /// Selects using the specified filter group.
        /// </summary>
        /// <param name="filterGroup">The filter group.</param>
        /// <returns></returns>
        public DbQuery Select(DbFilterGroup filterGroup)
        {
            return DbQuery.Create(this, filterGroup);
        }

        #endregion

        #region Relations

        /// <summary>
        /// Loads all records and process Child\Parent relations
        /// <example>
        /// <code>
        /// string query = @"
        ///         SELECT * FROM Users;
        ///         SELECT * FROM TasksAssignments;
        ///         SELECT * FROM Tasks;
        ///     ";
        /// 
        /// User[] users = gateway.LoadAndProcessRelations{User}(query, typeof(TasksAssignment), typeof(Task));
        /// 
        /// Task taks1 = users[0].TasksAssignments[0].Task);
        /// 
        /// 
        /// Types Definitions:
        /// 
        ///     [DbRecord]
        ///        public class TasksAssignment
        ///        {
        ///            [DbForeignKeyField(typeof(Task))]
        ///            public ulong TaskId;
        ///
        ///            [DbForeignKeyField(typeof(User))]
        ///            public ulong UserId;
        /// 
        ///            ...
        /// 
        ///            [DbParentRecord] public readonly Task Task; 
        ///            [DbParentRecord] public readonly User User;
        ///       }
        /// 
        ///     [DbRecord]
        ///     public class Task
        ///     {
        ///         [DbPrimaryKeyField]
        ///         public ulong Id;
        /// 
        ///         ...
        /// 
        ///         [DbChildRecords] public TasksAssignment[] TasksAssignment;
        ///     }
        /// 
        ///     [DbRecord]
        ///     public class User
        ///     {
        ///         [DbPrimaryKeyField]
        ///         public ulong Id;
        /// 
        ///         ...
        /// 
        ///         [DbChildRecords] public TasksAssignment[] TasksAssignments;
        ///     }
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">query which returns several records sets</param>
        /// <param name="dependenciesTypes">Related types to map</param>
        /// <returns>Array of records</returns>
        public T[] LoadAndProcessRelations<T>(string query, params Type[] dependenciesTypes)
        {
            using (IDataReader reader = Accessor.ExecuteReader(query))
            {
                DbDependenciesResolver<T> resolver = new DbDependenciesResolver<T>(reader, dependenciesTypes);
                return resolver.Root;
            }
        }

        /// <summary>
        /// Load records using association table
        /// </summary>
        /// <typeparam name="TargetType">Type we want to Load</typeparam>
        /// <typeparam name="AssociationType">Type which contains associations</typeparam>
        /// <param name="data">Data object</param>
        /// <returns>Array of Requested Objects</returns>
        /// <example>
        /// <code>
        /// Task []tasks = DbGateway.Instance.LoadAssociated{Task, TasksAssignment}(TestData.User);
        /// 
        /// Types Definitions:
        /// 
        ///     [DbRecord]
        ///        public class TasksAssignment
        ///        {
        ///            [DbForeignKeyField(typeof(Task))]
        ///            public ulong TaskId;
        ///
        ///            [DbForeignKeyField(typeof(User))]
        ///            public ulong UserId;
        /// 
        ///            ...
        ///       }
        /// 
        ///     [DbRecord]
        ///     public class Task
        ///     {
        ///         [DbPrimaryKeyField]
        ///         public ulong Id;
        /// 
        ///         ...
        ///     }
        /// 
        ///     [DbRecord]
        ///     public class User
        ///     {
        ///         [DbPrimaryKeyField]
        ///         public ulong Id;
        /// 
        ///         ...
        ///     }
        /// </code>
        /// </example>
        public TargetType[] LoadAssociated<TargetType, AssociationType>(object data)
        {
            Type primaryType = data.GetType();

            DbIdentityRecordInfo targetRecordInfo = DbAttributesManager.GetRecordInfo(typeof(TargetType)) as DbIdentityRecordInfo;
            if (targetRecordInfo == null)
                throw new NdbNotIdentityException("Only DbIdentityRecord objects can have related records");

            DbRecordInfo associationRecordInfo = DbAttributesManager.GetRecordInfo(typeof(AssociationType));

            DbIdentityRecordInfo sourceRecordInfo = DbAttributesManager.GetRecordInfo(primaryType) as DbIdentityRecordInfo;
            if (sourceRecordInfo == null)
                throw new NdbNotIdentityException("Only DbIdentityRecord objects can have related records");

            object primaryKey = sourceRecordInfo.PrimaryKey.GetValue(data);

            string select = DbQueryBuilder.BuildSelect(targetRecordInfo);
            // below is a self documented query? :)
            string sql = string.Format(
                @"{5} INNER JOIN {1} ON {0}.{3}={1}.{2} AND {1}.{4}=@PrimaryKey"
//                @"SELECT * FROM {0} INNER JOIN {1} ON {0}.{3}={1}.{2} AND {1}.{4}=@PrimaryKey"
                , targetRecordInfo.TableName
                , associationRecordInfo.TableName
                , associationRecordInfo.ForeignKeys[typeof(TargetType)].Name
                , targetRecordInfo.PrimaryKey.Name
                , associationRecordInfo.ForeignKeys[primaryType].Name
                , select
                );

            return loadRecords<TargetType>(targetRecordInfo, sql, "PrimaryKey", primaryKey);
        }

        /// <summary>
        /// Loads child records
        /// </summary>
        /// <typeparam name="ChildType">Type of the child objects</typeparam>
        /// <param name="data">Parent object</param>
        /// <param name="args">Filter</param>
        /// <returns>Array of childs</returns>
        /// <example>
        /// <code>
        /// Event[] events = DbGateway.Instance.LoadChilds{Event}(TestData.User);
        /// 
        /// Types Definitions:
        /// 
        ///    [DbRecord]
        ///    public class Event
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        ...
        ///     }
        /// 
        ///     [DbRecord]
        ///     public class User
        ///     {
        ///         [DbPrimaryKeyField]
        ///         public ulong Id;
        /// 
        ///         ...
        ///     }
        /// </code>
        /// </example>
        public ChildType[] LoadChilds<ChildType>(object data, params object [] args)
        {
            Type primaryType = data.GetType();

            DbRecordInfo childRecordInfo = DbAttributesManager.GetRecordInfo(typeof(ChildType));
            DbIdentityRecordInfo primaryRecordInfo = DbAttributesManager.GetRecordInfo(primaryType) as DbIdentityRecordInfo;

            if (primaryRecordInfo == null)
                throw new NdbNotIdentityException("Only DbIdentityRecord objects can have childs");

            object [] _args = UnionArgs(args,
                      childRecordInfo.ForeignKeys[primaryType].Name,
                      primaryRecordInfo.PrimaryKey.GetValue(data));

            return LoadList<ChildType>(_args);
        }

        /// <summary>
        /// Loads parent record
        /// </summary>
        /// <typeparam name="ParentType"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// User user = DbGateway.Instance.LoadParent{User}(TestData.WorkLog);
        /// 
        /// Types Definitions:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        [DbForeignKeyField(typeof(User))] 
        ///        public ulong UserId;
        /// 
        ///        ...
        ///     }
        /// 
        ///     [DbRecord]
        ///     public class User
        ///     {
        ///         [DbPrimaryKeyField]
        ///         public ulong Id;
        /// 
        ///         ...
        ///     }
        /// </code>
        /// </example>
        public ParentType LoadParent<ParentType>(object data)
        {
            Type childType = data.GetType();

            DbRecordInfo childRecordInfo = DbAttributesManager.GetRecordInfo(childType);

            if (!childRecordInfo.ForeignKeys.ContainsKey(typeof(ParentType)))
                throw new NdbException(string.Format(
                                        "The type '{0}' doesn't contains DbForeignKeyFieldAttribute to type '{1}'", childType, typeof(ParentType)));

            object primaryKey = childRecordInfo.ForeignKeys[typeof(ParentType)].GetValue(data);
            return Load<ParentType>(primaryKey);
        }

        #endregion


        #region Load

        /// <summary>
        /// Load an object by Primary Key from Database
        /// </summary>
        /// <param name="data">Object which has DbAttributes</param>
        /// <returns>true if requested object found in database</returns>
        /// <example>
        /// <code>
        /// WorkLog workLog = new WorkLog();
        /// workLog.Id = workLogId;
        /// bool success = DbGateway.Instance.Load(workLog);
        /// 
        /// Type Definition:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        ...
        ///     }
        /// </code>
        /// </example>
        public bool Load(object data)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(data.GetType());
            return load(data, info);
        }

        /// <summary>
        /// Load an object from database
        /// </summary>
        /// <param name="data">Object which has DbAttributes</param>
        /// <param name="args">Filter values</param>
        /// <returns>true if requested object found in database</returns>
        /// <example>
        /// <code>
        /// WorkLog workLog = new WorkLog();
        /// bool success = DbGateway.Instance.Load(workLog, "Date", DateTime.Now);
        /// 
        /// Type Definition:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        [DbField] 
        ///        public DateTime Date;
        ///        ...
        ///     }
        /// </code>
        /// </example>
        public bool Load(object data, params object[] args)
        {
            
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(data.GetType());
            
            using (IDataReader reader = Accessor.ExecuteReaderEx(DbQueryBuilder.BuildSelect(info), args))
            {
                if (reader.Read())
                {
                    Bind(data, reader, info);

                    if (reader.Read())
                        throw new NdbException(
                            "There are several records in database which match the specifyed filter");

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Load an object which has DbAttributes from database 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="primaryKey">Object Primary Key</param>
        /// <returns>object</returns>
        /// <example>
        /// <code>
        /// WorkLog workLog = DbGateway.Instance.Load{WorkLog}(workLogId);
        /// 
        /// Type Definition:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        ...
        ///     }
        /// </code>
        /// </example>
        /// <exception cref="NdbException">If Primary Key not found</exception>
        public T Load<T>(object primaryKey)
        {
            DbIdentityRecordInfo info = DbAttributesManager.GetRecordInfo(typeof(T)) as DbIdentityRecordInfo;
            if (info == null)
                throw new NdbNotIdentityException("Can't load record of type " + typeof(T));

            T data = (T) Activator.CreateInstance(typeof(T));

            info.PrimaryKey.SetValue(data, primaryKey);

            if (!load(data, info))
                return default(T);

            return data;
        }

        /// <summary>
        /// Loads record which matches the filter
        /// </summary>
        /// <typeparam name="T">Type to load</typeparam>
        /// <param name="args">Filter</param>
        /// <returns>Record or null if it doesn't exists</returns>
        public T Load<T>(params object[] args)
        {
            T data = (T)Activator.CreateInstance(typeof(T));
            
            if (Load(data, args))
                return data;

            return default(T);
        }

        /// <summary>
        /// Loads array of Objects
        /// </summary>
        /// <typeparam name="T">Type which has DbAttriibutes</typeparam>
        /// <param name="args">Filter values</param>
        /// <returns>Array of requested objects</returns>
        /// <example>
        /// <code>
        /// WorkLog [] list = DbGateway.Instance.LoadList{WorkLog}("Type", WorkLogType.Default);
        /// 
        /// Type Definition:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        [DbField] 
        ///        public WorkLogType Type;
        /// 
        ///        ...
        ///     }
        /// 
        ///     public enum WorkLogType : uint
        ///     {
        ///         Default,
        ///         ...
        ///     }
        /// </code>
        /// </example>
        public T[] LoadList<T>(params object[] args)
        {
            DbRecordInfo recordInfo = DbAttributesManager.GetRecordInfo(typeof(T));
            string query = DbAccessor.BuildWhere(DbQueryBuilder.BuildSelect(recordInfo), args);
            return LoadRecords<T>(query, args);
        }

        /// <summary>
        /// Loads limited array of Objects
        /// </summary>
        /// <typeparam name="T">Type which has DbAttriibutes</typeparam>
        /// <param name="limit">Objects Count Limit</param>
        /// <param name="args">Filter values</param>
        /// <returns>Array of requested objects</returns>
        /// <example>
        /// <code>
        /// WorkLog [] list = DbGateway.Instance.LoadList{WorkLog}(7, "Type", WorkLogType.Default);
        /// 
        /// Type Definition:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        [DbField] 
        ///        public WorkLogType Type;
        /// 
        ///        ...
        ///     }
        /// 
        ///     public enum WorkLogType : uint
        ///     {
        ///         Default,
        ///         ...
        ///     }
        /// </code>
        /// </example>
        public T[] LoadListLimited<T>(int limit, params object[] args)
        {
            return LoadListLimited<T>(limit, 0, args);
        }

        /// <summary>
        /// Loads limited array of Objects
        /// </summary>
        /// <typeparam name="T">Type which has DbAttriibutes</typeparam>
        /// <param name="limit">Objects Count Limit</param>
        /// <param name="offset">Objects Offset</param>
        /// <param name="args">Filter values</param>
        /// <returns>Array of requested objects</returns>
        /// <example>
        /// <code>
        /// WorkLog [] list = DbGateway.Instance.LoadList{WorkLog}(7, 7, "Type", WorkLogType.Default);
        /// 
        /// Type Definition:
        /// 
        ///    [DbRecord]
        ///    public class WorkLog
        ///    {
        ///        [DbPrimaryKeyField]
        ///        public ulong Id;
        ///         
        ///        [DbField] 
        ///        public WorkLogType Type;
        /// 
        ///        ...
        ///     }
        /// 
        ///     public enum WorkLogType : uint
        ///     {
        ///         Default,
        ///         ...
        ///     }
        /// </code>
        /// </example>
        public T[] LoadListLimited<T>(int limit, int offset, params object[] args)
        {
            DbRecordInfo recordInfo = DbAttributesManager.GetRecordInfo(typeof(T));
            string query = DbAccessor.BuildWhere(DbQueryBuilder.BuildSelect(recordInfo), args);
            return LoadRecords<T>(query, limit, offset, args);
        }

        /// <summary>
        /// Low level records loader
        /// </summary>
        /// <typeparam name="T">Type which has DbAttriibutes</typeparam>
        /// <param name="query">SQL query</param>
        /// <param name="args">filter values</param>
        /// <returns>Array of requested objects</returns>
        /// <example>
        /// <code>
        /// ViewRecordType[] list = DbGateway.Instance.LoadRecords{ViewRecordType}("SELECT ***.... ", "UserId", UserId)
        /// </code>
        /// </example>
        public T[] LoadRecords<T>(string query, params object[] args)
        {
            return LoadRecords<T>(query, 0, 0, args);
        }

        /// <summary>
        /// Low level records loader
        /// </summary>
        /// <typeparam name="T">Type which has DbAttriibutes</typeparam>
        /// <param name="query">SQL query</param>
        /// <param name="limit">Objects Count Limit</param>
        /// <param name="args">filter values</param>
        /// <returns>Array of requested objects</returns>
        /// <example>
        /// <code>
        /// ViewRecordType[] list = DbGateway.Instance.LoadRecords{ViewRecordType}("SELECT ***.... ", 7, "UserId", UserId)
        /// </code>
        /// </example>
        public T[] LoadRecords<T>(string query, int limit, params object[] args)
        {
            return LoadRecords<T>(query, limit, 0, args);
        }

        /// <summary>
        /// Low level records loader
        /// </summary>
        /// <typeparam name="T">Type which has DbAttriibutes</typeparam>
        /// <param name="query">SQL query</param>
        /// <param name="limit">Objects Count Limit</param>
        /// <param name="offset">Objects Offset</param>
        /// <param name="args">filter values</param>
        /// <returns>Array of requested objects</returns>
        /// <example>
        /// <code>
        /// ViewRecordType[] list = DbGateway.Instance.LoadRecords{ViewRecordType}("SELECT ***.... ", 7, 7, "UserId", UserId)
        /// </code>
        /// </example>
        public T[] LoadRecords<T>(string query, int limit, int offset, params object[] args)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(typeof(T));
            return loadRecords<T>(info, query, limit, offset, args);
        }

        /// <summary>
        /// Load results as key\value pairs
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">SQL query</param>
        /// <param name="key">Key Column Name</param>
        /// <param name="value">Value Column Name</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// Dictionary{int, string} fields = DbGateway.Instance.LoadKeyValue{string, string}(
        ///     "SELECT 7 as CustomKey, 'test2' as CustomValue", "CustomKey", "CustomValue");
        /// </code>
        /// </example>
        public Dictionary<TKey, TValue> LoadKeyValue<TKey, TValue>(string query, string key, string value)
        {
            var values = new Dictionary<TKey, TValue>();
            using (IDataReader reader = Accessor.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    values.Add((TKey)reader[key], (TValue)reader[value]);
                }
            }
            return values;
        }

        /// <summary>
        /// Loads the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public T[] LoadArray<T>(string query, string column)
        {
            var list = new List<T>();
            using (IDataReader reader = Accessor.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    list.Add((T)reader[column]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Loads the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public T LoadResult<T>(string query, params object [] args)
        {
            object value = Accessor.ExecuteScalar(query, args);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        #endregion


        #region Managment

        /// <summary>
        /// Stores object to database. If objects has PrimaryKey then sets it to new value
        /// </summary>
        /// <param name="data">an object with DbAttributes</param>
        public void Save(object data)
        {
            var info = DbAttributesManager.GetRecordInfo(data.GetType());

            DbIdentityRecordInfo identityInfo = info as DbIdentityRecordInfo;

            if (identityInfo != null)
            {
                save(data, identityInfo);
            }
            else
            {
                Accessor.Insert(info.TableName, info.GetValues(data));
            }
        }

        /// <summary>
        /// Saves the specified items to database.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Save(object [] items)
        {
            if (items == null || items.Length == 0)
                return;

            var info = DbAttributesManager.GetRecordInfo(items[0].GetType());

            DbIdentityRecordInfo identityInfo = info as DbIdentityRecordInfo;

            if (identityInfo != null)
            {
                foreach (object item in items)
                {
                    save(item, identityInfo);    
                }
            }
            else
            {
                foreach (object item in items)
                {
                    Accessor.Insert(info.TableName, info.GetValues(item));
                }
            }
                
        }

        /// <summary>
        /// Insert new object to database (if primary key was set, it will be overwrited)
        /// </summary>
        /// <param name="data"></param>
        public void Insert(object data)
        {
            var info = DbAttributesManager.GetRecordInfo(data.GetType());

            if (info is DbIdentityRecordInfo)
                insert(info as DbIdentityRecordInfo, data);
            else
                Accessor.Insert(info.TableName, info.GetValues(data));
        }

        /// <summary>
        /// Imports all fields of passed objects to mapped tables\columns in database (Primary Keys and other generated content).
        /// </summary>
        /// <param name="records">The records.</param>
        public void Import(object [] records)
        {
            if (records == null || records.Length == 0)
                return;

            var info = DbAttributesManager.GetRecordInfo(records[0].GetType());
            for (int i = 0; i < records.Length; i++)
            {
                Accessor.Insert(info.TableName, info.GetValues(records));    
            }
        }

        /// <summary>
        /// Imports all fields of passed DbRecord to database (Primary Keys etc).
        /// </summary>
        /// <param name="record">The record.</param>
        public void Import(object record)
        {
            var info = DbAttributesManager.GetRecordInfo(record.GetType());
            Accessor.Insert(info.TableName, info.GetValues(record));
        }

        /// <summary>
        /// Updates object in database.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if one object was updated</returns>
        public bool Update(object data)
        {
            var info = DbAttributesManager.GetRecordInfo(data.GetType());

            if (info is DbIdentityRecordInfo)
                return 1 == update(info as DbIdentityRecordInfo, data);

            throw new NdbException(string.Format(
                        "DbPrimaryKeyField attribute wasn't specifyed on {0} type", data.GetType()));
        }

        /// <summary>
        /// Updates objects in database by specifyed filter
        /// </summary>
        /// <param name="data">object with data to set</param>
        /// <param name="args">filter</param>
        /// <returns></returns>
        public int Update(object data, params object[] args)
        {
            var info = DbAttributesManager.GetRecordInfo(data.GetType());

            return Accessor.Update(info.TableName, info.GetValues(data), args);
        }

        /// <summary>
        /// Updates all entities of the specifyed type which match the passed args
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldsToUpdate"></param>
        /// <param name="args"></param>
        /// <returns>Count of updated objects</returns>
        public int Update(Type type, object [] fieldsToUpdate, params object[] args)
        {
            var info = DbAttributesManager.GetRecordInfo(type);

            return Accessor.Update(info.TableName, fieldsToUpdate, args);
        }

        /// <summary>
        /// Removes objects from database
        /// </summary>
        /// <param name="type">Type which has DbAttributes</param>
        /// <param name="args">filter values</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// uint count = DbGateway.Instance.Delete(typeof(WorkLog), "Type", WorkLogType.Default);
        /// </code>
        /// </example>
        public uint Delete(Type type, params object[] args)
        {
            string tableName = DbAttributesManager.GetTableName(type);
            return Accessor.Delete(tableName, args);
        }

        /// <summary>
        /// Remove object from database
        /// </summary>
        /// <param name="data">an object with DbAttributes</param>
        /// <returns>true if one object has been removed</returns>
        public bool Delete(object data)
        {
            var info = DbAttributesManager.GetRecordInfo(data.GetType());

            var identityRecordInfo = info as DbIdentityRecordInfo;
            if (identityRecordInfo != null)
            {
                DbFieldInfo primaryKey = identityRecordInfo.PrimaryKey;

                return 0 < Accessor.Delete(
                                info.TableName,
                                primaryKey.Name,
                                primaryKey.GetValue(data));
            }

            return 1 < DeleteByAllFields(data);
        }

        /// <summary>
        /// Remove records filtered by all field marked this DbFieldAttribute (use exact matching)
        /// </summary>
        /// <param name="data">an object with DbAttributes</param>
        /// <returns>Removed Records Count</returns>
        public uint DeleteByAllFields(object data)
        {
            var info = DbAttributesManager.GetRecordInfo(data.GetType());

            return Accessor.Delete(info.TableName, info.GetValues(data));
        }


        #endregion


        #region Helpers

        /// <summary>
        /// Loads Count of Records
        /// </summary>
        /// <param name="type">Type to load</param>
        /// <returns>Records Count</returns>
        public ulong LoadCount(Type type)
        {
            return Accessor.LoadCount(DbAttributesManager.GetTableName(type));
        }

        #endregion


        #endregion

        #region Private Helpers

        private static object[] UnionArgs(object[] other, params object[] args)
        {
            if (other.Length == 0) //ie there are no extra params
                return args;

            object[] _args = new object[other.Length + args.Length];
            args.CopyTo(_args, 0);
            other.CopyTo(_args, args.Length);

            return _args;
        }

        /// <summary>
        /// Binds the specified row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static T Bind<T>(IDataRecord row)
        {
            T obj = Activator.CreateInstance<T>();
            Bind(obj, row, typeof(T));
            return obj;
        }

        /// <summary>
        /// Binds the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="row">The row.</param>
        public static void Bind(object obj, IDataRecord row)
        {
            Bind(obj, row, obj.GetType());
        }

        private static void Bind(object data, IDataRecord row, Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            Bind(data, row, info);
        }

        private static void Bind(object data, IDataRecord row, DbRecordInfo info)
        {
//            int i = 0;

            var identityRecordInfo = info as DbIdentityRecordInfo;
            if (identityRecordInfo != null)
            {
                DbFieldInfo field = identityRecordInfo.PrimaryKey;

                object value = row[field.Name];
                if (value == DBNull.Value)
                {
                    if (field.DefaultValue != null)
                        value = field.DefaultValue;
                }
                else
                    value = DbConvertor.GetValue(field, value);

                setValue(field, data, value);

//                i++;
            }

            int count = info.Fields.Length;
            for (int j = 0; j < count; j++)
            {
                DbFieldInfo field = info.Fields[j];
                object value = row[field.Name];
                if (value == DBNull.Value)
                {
                    if (field.DefaultValue != null)
                        value = field.DefaultValue;
                }
                else
                    value = DbConvertor.GetValue(field, value);

                setValue(field, data, value);

//                i++;
            }
        }

        private static void setValue(DbFieldInfo field, object data, object value)
        {
            try
            {
                if (value == DBNull.Value)
                    field.SetValue(data, null);
                else
                {
                    Type type = DbConvertor.GetType(field.FieldType);                    
                    
                    object convertedValue = Convert.ChangeType(value, type);
                    field.SetValue(data, convertedValue);
                }
            }
            catch (Exception ex)
            {
                throw new NdbException("Can't set field value.\r\nField: " + field.Name + "\r\nError: " + ex.Message);
            }
        }

        private void save(object data, DbIdentityRecordInfo info)
        {
            if (!info.IsPrimaryKeyValid(data))
                insert(info, data);
            else
                update(info, data);
        }

        private bool load(object data, DbRecordInfo info)
        {
            if (info is DbIdentityRecordInfo)
                return load(data, info as DbIdentityRecordInfo);

            throw new NdbNotIdentityException("Can't load record");
        }

        private bool load(object data, DbIdentityRecordInfo info)
        {
            if (!info.IsPrimaryKeyValid(data))
                throw new NdbException("Primary Key wasn't set for object " + data.GetType());

            return Load(data, info.PrimaryKey.Name, info.PrimaryKey.GetValue(data));
        }

        private T[] loadRecords<T>(DbRecordInfo info, string query, params object[] args)
        {
            return loadRecords<T>(info, query, 0, 0, args);
        }

        private T[] loadRecords<T>(DbRecordInfo info, string query, int limit, int offset, params object[] args)
        {
            if (limit != 0 || offset != 0)
            {
                query = Accessor.BuildLimits(query, limit, offset);
            }
            
            using (IDataReader reader = Accessor.ExecuteReader(query, args))
            {
                return loadRecords<T>(reader, info);
            }
        }

        private static T[] loadRecords<T>(IDataReader reader, DbRecordInfo info)
        {
            List<T> list = new List<T>();
            while (reader.Read())
            {
                T data = Activator.CreateInstance<T>();
                Bind(data, reader, info);
                list.Add(data);
            }
            return list.ToArray();
        }

        internal static Array LoadRecords(Type type, IDataReader reader, DbRecordInfo info)
        {
            ArrayList list = new ArrayList();
            while (reader.Read())
            {
                object data = Activator.CreateInstance(type);
                Bind(data, reader, info);
                list.Add(data);
            }
            return list.ToArray(type);
        }

        private void insert(DbIdentityRecordInfo info, object data)
        {
            DbFieldInfo primaryKey = info.PrimaryKey;
            if (primaryKey.FieldType == typeof(Guid))
            {
                Guid guid = Guid.NewGuid();
                primaryKey.SetValue(data, guid);

                object[] values = info.GetValues(data, primaryKey.Name, guid);
                Accessor.Insert(info.TableName, values);
            }
            else
            {
                object[] values = info.GetValues(data);
                object newId = Accessor.InsertIdentity(info.TableName, primaryKey.Name, values);
                primaryKey.SetValue(data, Convert.ChangeType(newId, primaryKey.FieldType));
            }
        }

        private int update(DbIdentityRecordInfo info, object data)
        {
            if (!info.IsPrimaryKeyValid(data))
                throw new NdbException(string.Format(
                    "Primary Key wasn't set for the {0} object", data.GetType()));

            return Accessor.Update(info.TableName, info.GetValues(data),
                             info.PrimaryKey.Name, info.PrimaryKey.GetValue(data));
        }

        #endregion

        
    }
}