using System.Text;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Query
{
    internal class DbQueryBuilder
    {
        private DbAccessor dbAccessor;

        internal DbQueryBuilder(DbAccessor accessor)
        {
            dbAccessor = accessor;
        }

        internal string BuildSelectCount(string tableName)
        {
            return string.Concat("SELECT COUNT(*) FROM ", dbAccessor.QuoteName(tableName));
        }

        internal void BuildSelectCount(StringBuilder sb, DbRecordInfo recordInfo)
        {
            sb.Append(BuildSelectCount(recordInfo.TableName));
        }

        internal string BuildSelect(DbRecordInfo recordInfo)
        {
            var builder = new StringBuilder();
            BuildSelect(builder, recordInfo);
            return builder.ToString();
        }

        internal void BuildSelect(StringBuilder sb, DbRecordInfo recordInfo)
        {
            sb.Append("SELECT ");

            var identityRecordInfo = recordInfo as DbIdentityRecordInfo;
            if (identityRecordInfo != null)
            {
                appendField(sb, identityRecordInfo.PrimaryKey.Name);
                
            }
            int length = recordInfo.Fields.Length;
            for (int i = 0; i < length; i++)
            {
                appendField(sb, recordInfo.Fields[i].Name);
            }
            sb.Remove(sb.Length - 1, 1);

            sb.Append(" FROM ");
            sb.Append(recordInfo.TableName);
        }

        private void appendField(StringBuilder sb, string field)
        {
            sb.Append(dbAccessor.QuoteName(field));
            sb.Append(',');
        }
    }
}
