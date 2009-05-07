using System.Text;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Query
{
    internal class DbQueryBuilder
    {
        internal static string BuildSelectCount(string tableName)
        {
            return string.Concat("SELECT COUNT(*) FROM ", tableName);
        }

        internal static void BuildSelectCount(StringBuilder sb, DbRecordInfo recordInfo)
        {
            sb.Append(BuildSelectCount(recordInfo.TableName));
        }

        internal static string BuildSelect(DbRecordInfo recordInfo)
        {
            var builder = new StringBuilder();
            BuildSelect(builder, recordInfo);
            return builder.ToString();
        }

        internal static void BuildSelect(StringBuilder sb, DbRecordInfo recordInfo)
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

        private static void appendField(StringBuilder sb, string field)
        {
            sb.Append(field);
            sb.Append(',');
        }
    }
}
