using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;
using Npgsql;

namespace ITCreatings.Ndb.Accessors
{
    /// <summary>
    /// Summary description for DbCommon
    /// </summary>
    internal class PostgreAccessor : DbAccessor
    {
        #region Core

        public override string BuildLimits(string query, int limit, int offset)
        {
            return string.Concat(query, " LIMIT ", limit.ToString(), " OFFSET ", offset.ToString());
        }

        protected override DbCommand Command(string query)
        {
            return new NpgsqlCommand(query, new NpgsqlConnection(ConnectionString));
        }

        protected override DbDataAdapter GetAdapter()
        {
            return new NpgsqlDataAdapter();
        }

        #endregion

        #region DDL

        internal override string GetIdentity(string pk)
        {
            return " RETURNING " + pk;
        }

        #endregion

        #region SDL

        internal override Dictionary<string, string> LoadFields(Type type)
        {
            //TODO: load field length also

            DbGateway gateway = new DbGateway(this);

            return gateway.LoadKeyValue<string, string>(
                string.Format(@"SELECT a.attname AS field, t.typname AS type
                      FROM pg_class c, pg_attribute a, pg_type t
                     WHERE c.relname = '{0}'
                       AND a.attnum > 0
                       AND a.attrelid = c.oid
                       AND a.atttypid = t.oid
                       AND attname NOT IN ('cmin', 'cmax', 'ctid', 'oid', 'tableoid', 'xmin', 'xmax')
                     ORDER BY a.attnum", DbAttributesManager.GetTableName(type).ToLower()), "field", "type");
        }

        internal override string GetSqlType(Type type, uint size)
        {
            if (type == typeof(Byte))
                return "int2";

            if (type == typeof(Int32))
                return "int4";

            if (type == typeof(Int64))
                return "int8";

            if (type == typeof(Int16))
                return "int2";

            if (type == typeof(UInt32))
                return "int8";

            if (type == typeof(UInt64))
                return "int8";

            if (type == typeof(UInt16))
                return "int4";

            if (type == typeof(string))
            {
                if (size == 0)
                    size = 255;

                return string.Concat("varchar(", size, ")");
            }

            if (type == typeof(Guid))
                return "uuid";

            if (type == typeof(DateTime))
                return "timestamp";

            if (type == typeof(Double)) return "float";
            if (type == typeof(Decimal)) return "float";
            if (type == typeof(Boolean)) return "BOOLEAN";
            if (type == typeof(Byte[])) return "bytea";

            throw new NdbException("can't find Postgre type for the .NET Type - " + type);
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
                sb.AppendFormat("ADD COLUMN {0} {1} NULL,", item.Key, item.Value);
            }

            foreach (var item in checkResult.FieldsToUpdate)
            {
                sb.AppendFormat("ALTER COLUMN {0} TYPE {1},", item.Key, item.Value);
            }

            sb.Remove(sb.Length - 1, 1);
//            sb.Append(')');

            ExecuteNonQuery(sb.ToString());
        }

        internal override void CreateTable(DbRecordInfo info)
        {
            string postQueries = "";
            /*;
CREATE TABLE tablename (
    colname integer NOT NULL DEFAULT nextval('tablename_colname_seq')
);
ALTER SEQUENCE tablename_colname_seq OWNED BY tablename.colname;*/

            StringBuilder sb = new StringBuilder("CREATE TABLE " + info.TableName + " (");
            if (info is DbIdentityRecordInfo)
            {
                DbFieldInfo pk = (info as DbIdentityRecordInfo).PrimaryKey;

                string sequenceName = string.Format("{0}_{1}_seq", info.TableName, pk.Name);
                sb.Insert(0, "CREATE SEQUENCE " + sequenceName + ";");

                postQueries = string.Format("ALTER SEQUENCE {0}_{1}_seq OWNED BY {0}.{1};",
                                            info.TableName, pk.Name);
                sb.Append(getDefinition(pk) + " NOT NULL DEFAULT nextval('" + sequenceName + "'::regclass)");
                sb.Append(',');
            }

            foreach (DbFieldInfo field in info.Fields)
            {
                sb.Append(getDefinition(field));
                sb.Append(',');
            }

            string[] keys = getPrimaryKeys(info);
            sb.AppendFormat("PRIMARY KEY  ({0}), UNIQUE ({0})", String.Join(",", keys));
//            sb.AppendFormat("CONSTRAINT \"PK_{1}\" PRIMARY KEY  ({0})", String.Join(",", keys), info.TableName);
//            sb.AppendFormat("CONSTRAINT \"U_{1}\" UNIQUE ({0})", String.Join(",", keys), info.TableName);


            //Process indexes
            DbIndexesInfo indexes = DbAttributesManager.GetIndexes(info.Fields);

            ProcessIndexes(sb, indexes.Unique, ",UNIQUE ({0})");
//            ProcessIndexes(sb, indexes.Indexes, ",KEY {1} ({0})");
//            ProcessIndexes(sb, indexes.FullText, ",FULLTEXT KEY {1} ({0})");

            //process foreign keys
            foreach (KeyValuePair<Type, DbFieldInfo> key in info.ForeignKeys)
            {
                DbIdentityRecordInfo ri = DbAttributesManager.GetRecordInfo(key.Key) as DbIdentityRecordInfo;
                if (ri == null)
                    throw new NdbException("Only DbIdentityRecord objects can be used as Foreign Keys");

                sb.AppendFormat(
                    ",FOREIGN KEY ({1}) REFERENCES {0} ({2}) ON DELETE CASCADE ON UPDATE CASCADE"
                    , ri.TableName
                    , key.Value.Name
                    , ri.PrimaryKey.Name);
            }

            sb.Append(");");

            sb.Append(postQueries);

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