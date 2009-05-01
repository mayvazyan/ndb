using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlServerCe;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Accessors
{
    internal class MsSqlCeAccessor : DbAccessor
    {
        public override string BuildLimits(string query, int limit, int offset)
        {
            throw new System.NotImplementedException();
        }

        protected override DbCommand Command(string query)
        {
            return new SqlCeCommand(query, new SqlCeConnection(ConnectionString));
        }

        protected override DbDataAdapter GetAdapter()
        {
            return new SqlCeDataAdapter();
        }

        internal override string GetIdentity(string pk)
        {
            return ";SELECT LAST_INSERT_ID()";
        }

        internal override string GetSqlType(Type type, uint size)
        {
            throw new NotImplementedException();
        }

        internal override Dictionary<string, string> LoadFields(Type type)
        {
            throw new NotImplementedException();
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