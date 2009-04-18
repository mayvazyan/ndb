using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Accessors
{
    internal class MsSqlAccessor : DbAccessor
    {
        protected override DbCommand Command(string query)
        {
            return new SqlCommand(query, new SqlConnection(ConnectionString));
        }

        protected override DbDataAdapter GetAdapter()
        {
            return new SqlDataAdapter();
        }

        internal override string GetIdentity(string pk)
        {
            return "LAST_INSERT_ID()";
        }

        internal override string GetSqlType(Type type)
        {
            throw new System.NotImplementedException();
        }

        internal override Dictionary<string, string> LoadFields(Type type)
        {
            throw new System.NotImplementedException();
        }

        internal override Type GetType(string desc)
        {
            throw new System.NotImplementedException();
        }

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
    }
}