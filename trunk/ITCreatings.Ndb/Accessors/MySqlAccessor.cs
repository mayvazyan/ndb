using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
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

        protected override DbCommand Command(string query)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            return new MySqlCommand(query, connection);
        }

        protected override DbDataAdapter GetAdapter()
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

        internal override Dictionary<string, string> LoadFields(Type type)
        {
            DbGateway gateway = new DbGateway(this);

            return gateway.LoadKeyValue<string, string>(
                "describe " + DbAttributesManager.GetTableName(type), "Field", "Type");
        }

        internal override string GetSqlType(Type type)
        {
            if (type == typeof(Byte))
                return "tinyint";

            if (type == typeof(Int32))
                return "int";

            if (type == typeof(Int64))
                return "bigint";

            if (type == typeof(Int16))
                return "smallint";

            if (type == typeof(UInt32))
                return "int unsigned";

            if (type == typeof(UInt64))
                return "bigint unsigned";

            if (type == typeof(UInt16))
                return "smallint unsigned";

            if (type == typeof(string))
                return "varchar(255)";

            if (type == typeof(Guid))
                return "char(36)";

            if (type == typeof(DateTime))   return "datetime";
            if (type == typeof(Double))     return "float";
            if (type == typeof(Decimal))    return "float";
            if (type == typeof(Boolean))    return "BOOLEAN";
            if (type == typeof(Byte[]))     return "BLOB";

            throw new NdbException("can't find MySql type for the .NET Type - " + type);
        }

        internal override Type GetType(string desc)
        {
            if (desc.StartsWith("int"))
                return (desc.EndsWith("unsigned") ? typeof(UInt32) : typeof(Int32));

            if (desc.StartsWith("bigint"))
                return (desc.EndsWith("unsigned") ? typeof(UInt64) : typeof(Int64));

            if (desc.StartsWith("smallint"))
                return (desc.EndsWith("unsigned") ? typeof(UInt16) : typeof(Int16));

            if (desc.StartsWith("tinyint"))
                return (desc.EndsWith("unsigned") ? typeof(Byte) : typeof(Byte));

            if (desc == "datetime")
                return typeof(DateTime);

            if (desc.StartsWith("char") || desc.StartsWith("varchar") || desc.StartsWith("text"))
                return typeof(string);

            throw new NdbException("can't find .NET type for MySQL Type " + desc);
        }

        public override bool DropTable(string TableName)
        {
            try
            {
                ExecuteNonQuery("DROP TABLE " + TableName);
                return true;
            }
            catch (NdbConnectionFailedException)
            {
                throw;
            }
#if DEBUG
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(
                                      "Can't delete table {0} error {1}", TableName, ex.Message));
            }
#else
            catch
            {
            }
#endif
            return false;
        }

        private static string[] getPrimaryKeys(DbRecordInfo info)
        {
            if (info is DbIdentityRecordInfo)
            {
                return new[] { (info as DbIdentityRecordInfo).PrimaryKey.Name };
            }

            List<string> list = new List<string>(info.Fields.Length);
            foreach (DbFieldInfo field in info.Fields)
            {
                list.Add(field.Name);
            }

            return list.ToArray();
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
            if (info is DbIdentityRecordInfo)
            {
                DbFieldInfo key = ((DbIdentityRecordInfo)info).PrimaryKey;
                if (key.FieldType == typeof(Guid))
                {
                    sb.Append(getDefinition(key) + " NOT NULL");
                }
                else
                    sb.Append(getDefinition(key) + " NOT NULL auto_increment");

                sb.Append(',');
            }

            foreach (DbFieldInfo field in info.Fields)
            {
                sb.Append(getDefinition(field));
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
//            try
//            {
                ExecuteNonQuery(query);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(string.Format(
//                                      "query {0} failed this error {1}", query, ex.Message));
//                throw;
//            }

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

        #endregion
    }
}