using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Accessors
{
    internal class MsSqlAccessor : DbAccessor
    {
        /*WITH results AS
(
SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate DESC, p.ProjectId DESC) AS Pager,
*
FROM dbo.Projects p
WHERE (p.Name LIKE ‘ñàéò%’ )
)
SELECT *
FROM results
WHERE Pager BETWEEN 2 AND 7*/
        public override string BuildLimits(string query, int limit, int offset)
        {
            throw new System.NotImplementedException();
        }

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

        internal override string GetSqlType(Type type, uint size)
        {
            throw new System.NotImplementedException();
        }

        internal override Dictionary<string, string> LoadFields(Type type)
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