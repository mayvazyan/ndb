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
        /// <param name="query"></param>
        /// <returns></returns>
        protected override DbCommand Command(string query)
        {
            return new OleDbCommand(query, new OleDbConnection(ConnectionString));
        }

        /// <summary>
        /// Creates DbDataAdapter for the active database
        /// </summary>
        /// <returns></returns>
        protected override DbDataAdapter GetAdapter()
        {
            return new OleDbDataAdapter();
        }

        /// <summary>
        /// Executes Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="args">Filter</param>
        /// <returns>DataReader</returns>
        public override IDataReader ExecuteReader(string query, params object[] args)
        {
            IDataReader reader = base.ExecuteReader(query, args);
            return new ExcelDataReader(reader);
        }

        internal override string GetIdentity(string pk)
        {
            throw new System.NotImplementedException();
        }

        internal override string GetSqlType(Type type, uint size)
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
        public override bool DropTable(string name)
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
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns></returns>
        public IDataReader GetReader(string sheetName)
        {
            string query = string.Format("SELECT * FROM [{0}$]", sheetName);
            return ExecuteReader(query);
        }
    }
}