using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;
using MySql.Data.MySqlClient;

namespace ITCreatings.Ndb.Accessors
{
    /// <summary>
    /// Summary description for DbCommon
    /// </summary>
    internal class MySqlAccessor : DbAccessor
    {
        #region Core

        protected override string Format(string sql)
        {
            return sql.Replace('@', '?');
        }

        public override string BuildLimits(string query, int limit, int offset)
        {
            return string.Concat(query, " LIMIT ", offset.ToString(), ",", limit.ToString());
        }

        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public override DbDataAdapter GetAdapter()
        {
            return new MySqlDataAdapter();
        }

        #endregion


        #region DDL

        internal override string GetIdentity(string pk)
        {
            return ";SELECT LAST_INSERT_ID()";
        }

        #endregion


        #region SDL

        internal override string[] LoadTables(DbGateway gateway)
        {
            return gateway.LoadArray<string>("SHOW TABLE STATUS", "Name");
        }

        internal override Dictionary<string, string> LoadFields(DbGateway gateway, string tableName)
        {
            return gateway.LoadKeyValue<string, string>(
                "describe " + tableName, "Field", "Type");
        }

        protected override string GetSqlType(Type type, uint size)
        {
            if (type == typeof(Byte))
                return "tinyint(4)";

            if (type == typeof(Int32))
                return "int(10)";

            if (type == typeof(Int64))
                return "bigint(20)";

            if (type == typeof(Int16))
                return "smallint(10)";

            if (type == typeof(UInt32))
                return "int(10) unsigned";

            if (type == typeof(UInt64))
                return "bigint(20) unsigned";

            if (type == typeof(UInt16))
                return "smallint(10) unsigned";

            if (type == typeof(String))
            {
                if (size < 256)
                    return string.Concat("varchar(", size, ")"); // use varchar to allow indexes

                return getSqlType("TEXT", size);
            }

            if (type == typeof(Byte[]))
            {
                return getSqlType("BLOB", size);
            }

            if (type == typeof(Guid)) return "char(36)";

            if (type == typeof(DateTime) || type == typeof(DateTime?))   return "datetime";
            if (type == typeof(Double))     return "float";
            if (type == typeof(Decimal))    return "float";
            if (type == typeof(Boolean))    return "BOOLEAN";

            throw new NdbUnsupportedColumnTypeException(Provider, type);
        }

        public const uint NORMAL_MINSIZE = 256;
        public const uint MEDUIM_MINSIZE = 65536;
        public const uint LONG_MINSIZE = 16777216;

        internal static string getSqlType(string type, uint size)
        {
            string prefix;

            if (size < NORMAL_MINSIZE)
                prefix = "TINY";
            else
            if (size < MEDUIM_MINSIZE)
                prefix = "";
            else
            if (size < LONG_MINSIZE)
                prefix = "MEDIUM";
            else
//            if (size < 4294967296)
                prefix = "LONG";
//            else
//                throw new NdbInvalidColumnSizeException(type, size);

            return string.Concat(prefix, type);
        }

        public override void DropTable(string TableName)
        {
            ExecuteNonQuery(string.Concat("DROP TABLE IF EXISTS ", TableName, " CASCADE"));
        }

        internal override void AlterTable(DbTableCheckResult checkResult)
        {
            // TODO: update indexes - remove indexes before alter, and recreate after

            StringBuilder sb = new StringBuilder("ALTER TABLE " + checkResult.TableName + " ");
            foreach (var item in checkResult.FieldsToCreate)
            {
                sb.AppendFormat("ADD COLUMN `{0}` {1} NULL,", item.Key, item.Value);
            }

            foreach (var item in checkResult.FieldsToUpdate)
            {
                sb.AppendFormat("CHANGE `{0}` `{0}` {1} NULL,", item.Key, item.Value);
            }
            sb.Remove(sb.Length - 1, 1);
//            sb.Append(')');

            ExecuteNonQuery(sb.ToString());
        }

        internal override void CreateTable(DbRecordInfo info)
        {
            StringBuilder sb = new StringBuilder("CREATE TABLE " + info.TableName + "(");
            DbIdentityRecordInfo identityRecordInfo = info as DbIdentityRecordInfo;
            if (identityRecordInfo != null)
            {
                DbFieldInfo key = identityRecordInfo.PrimaryKey;
                if (key.FieldType == typeof(Guid) || !identityRecordInfo.IsDbGeneratedPrimaryKey)
                {
                    sb.Append(GetDefinition(key) + " NOT NULL");
                }
                else
                    sb.Append(GetDefinition(key) + " NOT NULL auto_increment");

                sb.Append(',');
            }

            foreach (DbFieldInfo field in info.Fields)
            {
                sb.Append(GetDefinition(field));
                sb.Append(',');
            }

            string[] keys = getPrimaryKeys(info);
            sb.AppendFormat("PRIMARY KEY  ({0})", String.Join(",", keys));

            //Process indexes
            DbIndexesInfo indexes = DbAttributesManager.GetIndexes(info.Fields);

            ProcessIndexes(sb, indexes.Unique, ",UNIQUE KEY {1} ({0})");
            ProcessIndexes(sb, indexes.Indexes, ",KEY {1} ({0})");
            ProcessIndexes(sb, indexes.FullText, ",FULLTEXT KEY {1} ({0})");

            //process foreign keys
            foreach (KeyValuePair<Type, DbFieldInfo> key in info.ForeignKeys)
            {
                DbIdentityRecordInfo ri = DbAttributesManager.GetRecordInfo(key.Key) as DbIdentityRecordInfo;
                if (ri == null)
                    throw new NdbException("Only DbIdentityRecord objects can be used as Foreign Keys");

                sb.AppendFormat(
                    ",FOREIGN KEY (`{1}`) REFERENCES `{0}` (`{2}`) ON DELETE CASCADE ON UPDATE CASCADE"
                    , ri.TableName
                    , key.Value.Name
                    , ri.PrimaryKey.Name);
            }

            sb.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8");

            string query = sb.ToString();
            ExecuteNonQuery(query);
        }

        private static void ProcessIndexes(StringBuilder sb, 
            IEnumerable<KeyValuePair<string, List<string>>> indexes, string format)
        {
            foreach (var index in indexes)
            {
                sb.AppendFormat(format, 
                    string.Join(",", index.Value.ToArray()), index.Key
                    );
            }
        }

        private static string[] getPrimaryKeys(DbRecordInfo info)
        {
            DbIdentityRecordInfo identityRecordInfo = info as DbIdentityRecordInfo;
            if (identityRecordInfo != null)
            {
                return new[] { identityRecordInfo.PrimaryKey.Name };
            }

            List<string> list = new List<string>(info.Fields.Length);
            foreach (DbFieldInfo field in info.Fields)
            {
                list.Add(field.Name);
            }

            return list.ToArray();
        }

        #endregion
    }
}