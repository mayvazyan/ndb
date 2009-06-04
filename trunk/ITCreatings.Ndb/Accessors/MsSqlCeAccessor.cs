using System.Data.Common;
using System.Data.SqlServerCe;

namespace ITCreatings.Ndb.Accessors
{
    internal class MsSqlCeAccessor : MsSqlAccessor
    {
        protected override DbCommand Command(string query)
        {
            return new SqlCeCommand(query, new SqlCeConnection(ConnectionString));
        }

        public override DbDataAdapter GetAdapter()
        {
            return new SqlCeDataAdapter();
        }

        internal override string GetIdentity(string pk)
        {
            return ";SELECT LAST_INSERT_ID()";
        }
/*
        public override string BuildLimits(string query, int limit, int offset)
        {
            throw new System.NotImplementedException();
        }

        internal override string GetSqlType(Type type, uint size)
        {
            throw new NdbUnsupportedColumnTypeException(Provider, type);
        }

        internal override Dictionary<string, string> LoadFields(DbGateway gateway, string tableName)
        {
            throw new NotImplementedException();
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

        internal override string[] LoadTables(DbGateway gateway)
        {
            throw new System.NotImplementedException();
        }
 */
    }
}