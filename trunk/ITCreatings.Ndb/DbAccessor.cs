using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using ITCreatings.Ndb.Accessors;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb
{
    /// <summary>
    /// Contains low level functionality for working with database
    /// </summary>
    public abstract class DbAccessor
    {
        #region properties

        /// <summary>
        /// Is Instance is SqLite database
        /// </summary>
        public bool IsSqLite { get { return dbProvider == DbProvider.SqLite; } }

        /// <summary>
        /// Is Instance is Postgre database
        /// </summary>
        public bool IsPostgre { get { return dbProvider == DbProvider.Postgre; } }

        /// <summary>
        /// Is Instance is MySQL database
        /// </summary>
        public bool IsMySql { get { return dbProvider == DbProvider.MySql; } }

        #endregion

        #region Instance Logic

        //TODO: add multithread support?
        private static DbAccessor instance;

        /// <summary>
        /// Singleton Accessor
        /// </summary>
        public static DbAccessor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Create("NdbConnection");
                }
   
                return instance;
            }
        }

        /// <summary>
        /// Initializes Singleton Accessor
        /// </summary>
        /// <param name="dbProvider"></param>
        public static void InitInstance(DbProvider dbProvider)
        {
            instance = Create(dbProvider);
        }

        /// <summary>
        /// Initializes Singleton Accessor
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="connectionString"></param>
        public static void InitInstance(DbProvider dbProvider, string connectionString)
        {
            instance = Create(dbProvider, connectionString);
        }

        /// <summary>
        /// Loads ConnectionStringSettings from the config
        /// </summary>
        /// <param name="connectionStringName">Key of the config value</param>
        /// <returns>DbAccessor</returns>
        public static DbAccessor Create(string connectionStringName)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (s == null)
                throw new NdbConnectionFailedException(string.Format(
                    "ConnectionString `{0}` wasn't set", connectionStringName));

            DbProvider provider = getProviderFromConfig(s);
            return Create(provider, s.ConnectionString);
        }

        /// <summary>
        /// Creates an database accessor for the specifyed database type
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public static DbAccessor Create(DbProvider dbProvider)
        {
            DbAccessor accessor;

            switch (dbProvider)
            {
                case Ndb.DbProvider.MySql:
                    accessor = new MySqlAccessor();
                    break;

                case Ndb.DbProvider.SqLite:
                    accessor = new SqLiteAccessor();
                    break;

                case Ndb.DbProvider.Postgre:
                    accessor = new PostgreAccessor();
                    break;

                case Ndb.DbProvider.MsSql:
                    accessor = new MsSqlAccessor();
                    break;

                case Ndb.DbProvider.MsSqlCe:
                    accessor = new MsSqlCeAccessor();
                    break;

                default:
                    throw new NdbConnectionFailedException(string.Format("Provider {0} doesn't supported", dbProvider));
            }

            accessor.dbProvider = dbProvider;
            
            return accessor;
        }

        /// <summary>
        /// Creates an database accessor
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbAccessor Create(DbProvider dbProvider, string connectionString)
        {
            DbAccessor accessor = Create(dbProvider);
            accessor.ConnectionString = connectionString;

            return accessor;
        }

        #endregion

        #region Base Logic

        private DbProvider dbProvider;

        /// <summary>
        /// Returns current database provider
        /// </summary>
        public DbProvider DbProvider
        {
            get
            {
                return dbProvider;
            }
            set
            {
                dbProvider = value;
            }
        }

        private static DbProvider getProviderFromConfig(ConnectionStringSettings connection)
        {
            try
            {
                return (DbProvider)Enum.Parse(typeof(DbProvider), connection.ProviderName);
            }
            catch
            {
                throw new NdbConnectionFailedException(string.Format("Provider {0} doesn't supported", connection.ProviderName));
            }
        }

        private string connectionString;

        /// <summary>
        /// By default will be loaded from the config - "NdbConnection" connection string
        /// Also can be set programmatically
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new NdbConnectionFailedException("Connection String wasn't set");
                }

                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        #endregion

        /// <summary>
        /// change @ to appropriative char (ex ? for MySql)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected virtual string Format(string sql)
        {
            return sql;
        }

        /// <summary>
        /// Adds limit to query
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <param name="limit">Records Limit</param>
        /// <param name="offset">Records Offset</param>
        /// <returns></returns>
        public abstract string BuildLimits(string query, int limit, int offset);

        #region Command

        /// <summary>
        /// Creates DbCommand for active database
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract DbCommand Command(string query);

        /// <summary>
        /// Creates DbCommand for passed query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        protected virtual DbCommand Command(string query, params object[] par)
        {
            string formatedQuery = Format(query);
            
            Debug.WriteLine(formatedQuery);

            var command = Command(formatedQuery);

            for (int i = 0; i < par.Length; i += 2)
            {
                if (!string.IsNullOrEmpty(par[i].ToString()))
                {
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = Format(par[i].ToString());
                    parameter.Value = par[i + 1];
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        private static void CommandDispose(DbCommand command)
        {
            if (command != null)
            {
                if (command.Connection != null)
                {
                    command.Connection.Close();
                    command.Connection.Dispose();
                }
                command.Dispose();
            }
        }

        private DbCommand CommandEx(string query, params object [] args)
        {
            var command = Command(query, args);

            try
            {
                command.Connection.Open();
            }
            catch (Exception ex)
            {
                throw new NdbConnectionFailedException(command.Connection.ConnectionString, ex);
            }

            return command;
        }

        #endregion

        #region DataAdapter

        /// <summary>
        /// Creates DbDataAdapter for the active database
        /// </summary>
        /// <returns></returns>
        protected abstract DbDataAdapter GetAdapter();

        /// <summary>
        /// Creates DbDataAdapter for the passed query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        protected DbDataAdapter DataAdapter(string query, params object[] par)
        {
            DbDataAdapter adapter = GetAdapter();
            adapter.SelectCommand = Command(query, par);
            return adapter;
        }

        #endregion

        #region DataSet

        /// <summary>
        /// Load Data Set. Use this method, if you need load 2 or more tables from 1 query
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="args">if need, you may load params</param>
        /// <returns></returns>
        public DataSet LoadDataSet(string query, params object[] args)
        {
            return LoadDataSet(query, 0, 0, args);
        }

        /// <summary>
        /// Load Data Set. Use this method, if you need load 2 or more tables from 1 query
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="limit">count limit</param>
        /// <param name="args">if need, you may load params</param>
        /// <returns></returns>
        public DataSet LoadDataSet(string query, int limit, params object[] args)
        {
            return LoadDataSet(query, limit, 0, args);
        }

        /// <summary>
        /// Load Data Set. Use this method, if you need load 2 or more tables from 1 query
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="limit">records count limit</param>
        /// <param name="offset">records offset</param>
        /// <param name="args">if need, you may load params</param>
        /// <returns></returns>
        public DataSet LoadDataSet(string query, int limit, int offset, params object[] args)
        {
            if (limit != 0 || offset != 0)
            {
                query = BuildLimits(query, limit, offset);
            }

            DataSet set = new DataSet();
            DbDataAdapter adapter = null;
            try
            {   
                adapter = DataAdapter(query, args);
                adapter.Fill(set);
                return set;
            }
            catch (Exception ex)
            {
                throw new NdbException("Unable Fill Data Set", ex);
            }
            finally
            {
                if (adapter != null)
                {
                    if (adapter.SelectCommand != null)
                    {
                        if (adapter.SelectCommand.Connection != null)
                        {
                            adapter.SelectCommand.Connection.Close();
                            adapter.SelectCommand.Connection.Dispose();
                        }
                        adapter.SelectCommand.Dispose();
                    }
                    adapter.Dispose();
                }
            }
        }
        #endregion DataSet

        #region DataTable

        /// <summary>
        /// Loads Data Table
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <param name="args">Filters</param>
        /// <returns></returns>
        public DataTable LoadDataTable(string query, params object[] args)
        {
            return LoadDataTable(query, 0, 0, args);
        }

        /// <summary>
        /// Loads Data Table
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <param name="limit">Records Limit</param>
        /// <param name="args">Filters</param>
        /// <returns></returns>
        public DataTable LoadDataTable(string query, int limit, params object[] args)
        {
            return LoadDataTable(query, limit, 0, args);
        }

        /// <summary>
        /// Loads Data Table
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <param name="limit">Records Limit</param>
        /// <param name="offset">Records Offset</param>
        /// <param name="args">Filters</param>
        /// <returns></returns>
        public DataTable LoadDataTable(string query, int limit, int offset, params object[] args)
        {
            DataSet set = LoadDataSet(query, limit, offset, args);
            
            if (set.Tables.Count > 0)
                return set.Tables[0];

            throw new NdbException("There are no Tables Loaded by query: " + query);
        }

        #endregion
        
        #region NonQuery

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="args">parameters, if need</param>
        /// <returns>Affected rows</returns>
        public int ExecuteNonQuery(string query, params object[] args)
        {
            DbCommand command = null;
            try
            {
                command = CommandEx(query, args);

                return command.ExecuteNonQuery();
            }
            catch (NdbConnectionFailedException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new NdbException("Unable Execute Non Query:\r\n" + query, ex);
            }
            finally
            {
                CommandDispose(command);
            }
        }

        #endregion NonQuery

        #region Scalar
        
        /// <summary>
        /// Executes Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="args">Filter</param>
        /// <returns>Scalar result</returns>
        public object ExecuteScalar(string query, params object[] args)
        {
            DbCommand command = null;
            try
            {
                command = CommandEx(query, args);
                
                return command.ExecuteScalar();
            }
            catch (NdbConnectionFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NdbException("Cannot execute query\r\n:" + query, ex);
            }
            finally
            {
                CommandDispose(command);
            }
        }
        #endregion Scalar

        #region Reader

        /// <summary>
        /// Executes Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="args">Filter</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteReader(string query, params object[] args)
        {
            DbCommand command = null;
            try
            {
                command = CommandEx(query, args);
                
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (NdbConnectionFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                CommandDispose(command);

                throw new NdbException("Execute Reader failed", ex);
            }
        }

        #endregion Reader

        #region Utils Methods

        internal abstract string GetIdentity(string pk);
//        internal abstract string ExpressionToString(DbExpressionType expressionType);

        private object ExecuteInsert(string query, string pk, params object[] args)
        {
            return ExecuteScalar(query + GetIdentity(pk), args);
        }

        internal static string BuildWhere(string baseQuery, object[] args)
        {
            StringBuilder sb = new StringBuilder(baseQuery + " WHERE ");
            for (int i = 0; i < args.Length; i += 2)
            {
                sb.Append(args[i]);
                sb.Append('=');
                sb.Append('@');
                sb.Append(args[i]);
                sb.Append(" AND ");
            }
            return sb.ToString(0, sb.Length - 5);
        }

        private static string BuildInsertQuery(string tableName, object[] args)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO " + tableName);

            StringBuilder columnsSb = new StringBuilder();
            StringBuilder valuesSb = new StringBuilder();

            for (int i = 0; i < args.Length; i += 2)
            {
                columnsSb.Append(args[i]);
                columnsSb.Append(',');

                valuesSb.Append('@');
                valuesSb.Append(args[i]);
                valuesSb.Append(',');
            }

            sb.AppendFormat(" ({0}) VALUES ({1})",
                columnsSb.ToString().TrimEnd(','), valuesSb.ToString().TrimEnd(','));

            return sb.ToString();
        }

        #endregion

        #region SDL

        internal string GetSqlType(Type type)
        {
            return GetSqlType(type, 0);
        }

        internal abstract string GetSqlType(Type type, uint size);
        internal abstract Dictionary<string, string> LoadFields(Type type);

        internal virtual Type GetType(string desc)
        {
            throw new NdbException("this method wasn't overloaded provider: " + dbProvider);
        }



        /// <summary>
        /// Drops Table from Database
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if success</returns>
        public abstract bool DropTable(string name);

        internal abstract void AlterTable(DbTableCheckResult checkResult);
        internal abstract void CreateTable(DbRecordInfo info);

        /// <summary>
        /// Gets SQL definition for a specifyed field
        /// </summary>
        /// <param name="field"></param>
        /// <returns>SDL column definition</returns>
        protected string getDefinition(DbFieldInfo field)
        {
            StringBuilder sb = new StringBuilder(field.Name);
            sb.Append(' ');

            Type type = field.FieldType;

            if (type.BaseType == typeof(Enum))
            {
                type = Enum.GetUnderlyingType(type);
            }
            
            sb.Append(GetSqlType(type, field.Size));
            return sb.ToString();
        }

        #endregion

        #region Auto Where Generating Methods

        /// <summary>
        /// Build Where clause and call <see cref="ExecuteReader"/>
        /// </summary>
        /// <example>
        /// <code>
        /// ExecuteReaderEx("SELECT * FROM TestTable", "TestColumn", "TestValue");
        /// </code>
        /// </example>
        /// <param name="query">SQL query without WHERE clause</param>
        /// <param name="args">Filter</param>
        /// <returns></returns>
        public IDataReader ExecuteReaderEx(string query, object[] args)
        {
            return ExecuteReader(BuildWhere(query, args), args);
        }

        /// <summary>
        /// Build Where clause and call <see cref="ExecuteNonQuery"/>
        /// </summary>
        /// <example>
        /// <code>
        /// ExecuteNonQueryEx("DELETE FROM TestTable", "TestColumn", "TestValue");
        /// </code>
        /// </example>
        /// <param name="query">SQL query without WHERE clause</param>
        /// <param name="args">Filter</param>
        /// <returns></returns>
        public int ExecuteNonQueryEx(string query, object[] args)
        {
            return ExecuteNonQuery(BuildWhere(query, args), args);
        }

        #endregion
        
        #region DDL

        /// <summary>
        /// removes all records which matchs filter args
        /// </summary>
        /// <example>
        /// <code>
        /// DbAccessor.Instance.Delete("MyTable", "Status", 2, "DateCreate", DateTime.Today)
        /// </code>
        /// </example>
        /// <param name="TableName">Target table</param>
        /// <param name="args">Filter</param>
        public uint Delete(string TableName, params object[] args)
        {
            return Convert.ToUInt32(ExecuteNonQueryEx("DELETE FROM " + TableName, args));
        }

        /// <summary>
        /// Updates table in database. Sets new values to records which match the args
        /// </summary>
        /// <param name="tableName">Database table name</param>
        /// <param name="values">Values to set</param>
        /// <param name="args">Filter</param>
        /// <returns>Affected rows count</returns>
        public int Update(string tableName, object[] values, params object[] args)
        {
            StringBuilder sb = new StringBuilder("UPDATE " + tableName + " SET ");

            for (int i = 0; i < values.Length; i += 2)
            {
                sb.Append(values[i]);
                sb.Append('=');

                sb.Append('@');
                sb.Append(values[i]);
                sb.Append(',');
            }

            sb.Remove(sb.Length - 1, 1);

            object[] _args = new object[values.Length + args.Length];
            values.CopyTo(_args, 0);
            args.CopyTo(_args, values.Length);

            string query = BuildWhere(sb.ToString(), args);
            return ExecuteNonQuery(query, _args);
        }


        /// <summary>
        /// Inserts a record into specifyed table
        /// 
        /// <example>
        /// <code>
        /// DbAccessor.Instance.InsertIdentity("Users", "Id", "Email", "empty@example.com", "Dob", DateTime.Now);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="tableName">Target table</param>
        /// <param name="autoIncrementColumnName">Auto increment column name</param>
        /// <param name="args">Values</param>
        /// <returns>Generated Id</returns>
        public object InsertIdentity(string tableName, string autoIncrementColumnName, params object[] args)
        {
            return ExecuteInsert(BuildInsertQuery(tableName, args), autoIncrementColumnName, args);
        }

        /// <summary>
        /// Insert a record into specifyed table
        /// </summary>
        /// <param name="tableName">Target table</param>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("This method doesn't works with Postgre SQL database")]
        public object InsertIdentity(string tableName, params object[] args)
        {
            return ExecuteInsert(BuildInsertQuery(tableName, args), string.Empty, args);
        }

        /// <summary>
        /// Inserts a record into specifyed table
        /// </summary>
        /// <param name="tableName">Target Table</param>
        /// <param name="args">Values</param>
        public void Insert(string tableName, params object[] args)
        {
            ExecuteNonQuery(BuildInsertQuery(tableName, args), args);
        }


        /// <summary>
        /// Loads Count of Records in the specifyed Table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>Count</returns>
        public ulong LoadCount(string tableName)
        {
            object scalar = ExecuteScalar("SELECT COUNT(*) FROM " + tableName);

            return Convert.ToUInt64(scalar);
        }

        /// <summary>
        /// Returns is accessor can connect to database
        /// </summary>
        public bool CanConnect
        {
            get
            {
                DbCommand command = null;
                try
                {
                    command = CommandEx(""); 
                }
                catch(NdbConnectionFailedException)
                {
                    return false;
                }
                finally
                {
                    CommandDispose(command);
                }
                return true;
            }
        }

        #endregion
    }
}