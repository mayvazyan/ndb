using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using ITCreatings.Ndb.Accessors.DataReaders;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Accessors
{
    /// <summary>
    /// Excel Accessor
    /// </summary>
    public class ExcelAccessor : DbAccessor
    {
        /// <summary>
        /// Creates DbCommand for active database
        /// </summary>
        /// <returns></returns>
        protected override DbConnection CreateConnection()
        {
            return new OleDbConnection(ConnectionString);
        }

        /// <summary>
        /// Creates DbDataAdapter for the active database
        /// </summary>
        /// <returns></returns>
        public override DbDataAdapter GetAdapter()
        {
            return new OleDbDataAdapter();
        }

        /// <summary>
        /// Executes Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="args">Filter</param>
        /// <returns>DataReader</returns>
        public override IDataReader ExecuteReader(string query, object[] args)
        {
            IDataReader reader = base.ExecuteReader(query, args);
            return new ExcelDataReader(reader);
        }

        internal override string GetIdentity(string pk)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetSqlType(Type type, uint size)
        {
            throw new System.NotImplementedException();
        }

        internal override Dictionary<string, string> LoadFields(DbGateway gateway, string tableName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Loads the tables.
        /// 
        /// Used in code generation purposes
        /// </summary>
        /// <returns></returns>
        internal override string[] LoadTables(DbGateway gateway)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Drops Table from Database
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if success</returns>
        public override void DropTable(string name)
        {
            throw new System.NotImplementedException();
        }

        internal override void AlterTable(DbTableCheckResult checkResult)
        {
            throw new System.NotImplementedException();
        }

        internal override void CreateTable(DbRecordInfo info)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds limit to query
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <param name="limit">Records Limit</param>
        /// <param name="offset">Records Offset</param>
        /// <returns></returns>
        public override string BuildLimits(string query, int limit, int offset)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the data reader of the specified Excel Sheet.
        /// <example>
        /// <code>
        /// using(IDataReader reader = excelAccessor.GetReader("Sheet1"))
        /// {
        ///     DoSomeStuff(reader);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns></returns>
        public IDataReader GetReader(string sheetName)
        {
            return GetReader(sheetName, string.Empty);
        }

        /// <summary>
        /// Gets the data reader of the specified Excel Sheet.
        /// <example>
        /// <code>
        /// using(IDataReader reader = excelAccessor.GetReader("Sheet1", "A1:B10"))
        /// {
        ///     DoSomeStuff(reader);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public IDataReader GetReader(string sheetName, string range)
        {
            string query = string.Format("SELECT * FROM [{0}${1}]", sheetName, range);
            return ExecuteReader(query);
        }

        /// <summary>
        /// Loads the list.
        /// <example>
        /// <code>
        /// User[] users = excelAccessor.LoadList&lt;User&gt;("Users Sheet");
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns></returns>
        public T[] LoadList<T>(string sheetName) where T : new()
        {
            using (IDataReader reader = GetReader(sheetName))
            {
                return DbGateway.LoadList<T>(reader);
            }
        }
        /// <summary>
        /// Loads the list.
        /// <example>
        /// 		<code>
        /// User[] users = excelAccessor.LoadList&lt;User&gt;("Users Sheet", "A1:B10");
        /// </code>
        /// 	</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public T[] LoadList<T>(string sheetName, string range) where T : new()
        {
            using (IDataReader reader = GetReader(sheetName, range))
            {
                return DbGateway.LoadList<T>(reader);
            }
        }

        /// <summary>
        /// Exports the specified sheet to passed gateway.
        /// <example>
        /// <code>
        /// var target = new DbGateway(DbAccessor.Create("SampleDb"));
        /// var source = (ExcelAccessor)DbAccessor.Create("SampleExcelFile");
        /// 
        /// source.Export&lt;Contact&gt;("Contacts", target);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetGateway">The target gateway.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        public void Export<T>(DbGateway targetGateway, string sheetName) where T : class, new()
        {
            T[] list = LoadList<T>(sheetName);
            targetGateway.Import(list);
        }

        /// <summary>
        /// Exports the specified sheet to passed gateway.
        /// <example>
        /// <code>
        /// var target = new DbGateway(DbAccessor.Create("SampleDb"));
        /// var source = (ExcelAccessor)DbAccessor.Create("SampleExcelFile");
        /// 
        /// source.Export&lt;Contact&gt;(target);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetGateway">The target gateway.</param>
        public void Export<T>(DbGateway targetGateway) where T : class, new()
        {
            string tableName = DbAttributesManager.GetTableName(typeof(T));
            Export<T>(targetGateway, tableName);
        }

        /// <summary>
        /// Exports the specified sheet to passed gateway and removes old data from passed gateway
        /// <example>
        /// <code>
        /// var target = new DbGateway(DbAccessor.Create("SampleDb"));
        /// var source = (ExcelAccessor)DbAccessor.Create("SampleExcelFile");
        /// 
        /// source.ExportWithClean&lt;Contact&gt;(target, "Contacts");
        /// </code>
        /// </example>
        /// </summary> 
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="targetGateway">The target gateway.</param>
        public void ExportWithClean<T>(DbGateway targetGateway, string sheetName) where T : class, new()
        {
            T[] list = LoadList<T>(sheetName);
            targetGateway.Delete(typeof (T));
            targetGateway.Import(list);
        }

        /// <summary>
        /// Exports the specified sheet to passed gateway and removes old data from passed gateway
        /// <example>
        /// <code>
        /// var target = new DbGateway(DbAccessor.Create("SampleDb"));
        /// var source = (ExcelAccessor)DbAccessor.Create("SampleExcelFile");
        /// 
        /// source.ExportWithClean&lt;Contact&gt;(target, "Contacts");
        /// </code>
        /// </example>
        /// </summary> 
        /// <typeparam name="T"></typeparam>
        /// <param name="targetGateway">The target gateway.</param>
        public void ExportWithClean<T>(DbGateway targetGateway) where T : class, new()
        {
            string tableName = DbAttributesManager.GetTableName(typeof(T));
            Export<T>(targetGateway, tableName);
        }
    }
}