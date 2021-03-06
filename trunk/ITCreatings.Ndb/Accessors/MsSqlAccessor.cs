﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Utils;

namespace ITCreatings.Ndb.Accessors
{
    internal class MsSqlAccessor : DbAccessor
    {
        protected override DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public override DbDataAdapter GetAdapter()
        {
            return new SqlDataAdapter();
        }

        internal override string GetIdentity(string pk)
        {
            return ";SELECT @@IDENTITY";
        }

        internal override string QuoteName(string name)
        {
            return string.Concat('[', name, ']');
        }

        protected override string GetSqlType(Type type, uint size)
        {
            if (type == typeof(Byte))
                return "tinyint";

            if (type == typeof(Int32))
                return "int";

            if (type == typeof(Int64))
                return "bigint";

            if (type == typeof(Int16))
                return "smallint";

            if (type == typeof(String))
            {
                if (size == 0)
                    size = 255;

                if (size < 4000)
                    return string.Concat("nvarchar(", size, ")");

                return "ntext";
            }

            if (type == typeof(Byte[]))
            {
                if (size > 8000)
                    throw new NdbInvalidColumnSizeException("varbinary", size);

                return string.Concat("varbinary(", size, ")"); // use varchar to allow indexes
            }

            if (type == typeof(Guid)) return "uniqueidentifier";
            if (type == typeof(DateTime) || type == typeof(DateTime?)) return "datetime";
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?)) return "time";
            if (type == typeof(Double)) return "float(8)";
            if (type == typeof(Decimal)) return "money";
            if (type == typeof(Boolean)) return "bit";
            if (type == typeof(Microsoft.SqlServer.Types.SqlGeography)) return "geography";

            throw new NdbUnsupportedColumnTypeException(Provider, type);
        }

        internal override Dictionary<string, string> LoadFields(DbGateway gateway, string tableName)
        {
            // thanks to Kristof Neirynck - http://www.crydust.be/blog/2006/12/26/mssql-lacks-describe-statement/
            return gateway.LoadKeyValue<string, string>(
                string.Format(
                    @"SELECT column_name as name,   data_type +
                        COALESCE(
                           '(' + CAST(character_maximum_length AS VARCHAR) + ')'
                        ,  '(' + CAST(numeric_precision AS VARCHAR) + ')'
                        ,  ''
                        ) as [type]
                    FROM
                        information_schema.columns
                    WHERE
                        table_name = N'{0}';", tableName),
                "name", "type");
        }

        public override void DropTable(string tableName)
        {
            ExecuteNonQuery(string.Format(
                @"IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}') DROP TABLE {0};",
                tableName));
        }

        internal override void AlterTable(DbTableCheckResult checkResult)
        {
            // TODO: (Ndb) update indexes - remove indexes before alter, and recreate after

            StringBuilder sb = new StringBuilder("ALTER TABLE " + checkResult.TableName + " ");
            foreach (var item in checkResult.FieldsToCreate)
            {
                sb.AppendFormat("ADD COLUMN [{0}] {1} NULL,", item.Key, item.Value);
            }

            foreach (var item in checkResult.FieldsToUpdate)
            {
                sb.AppendFormat("CHANGE [{0}] [{0}] {1} NULL,", item.Key, item.Value);
            }
            sb.Remove(sb.Length - 1, 1);

            ExecuteNonQuery(sb.ToString());
        }

        internal override void CreateTable(DbRecordInfo info)
        {
            StringBuilder sb = new StringBuilder("CREATE TABLE " + info.TableName + "(");
            DbIdentityRecordInfo identityRecordInfo = info as DbIdentityRecordInfo;
            if (identityRecordInfo != null)
            {
                DbFieldInfo key = identityRecordInfo.PrimaryKey;
                if (key.FieldType == typeof(Guid))
                {
                    sb.Append(GetDefinition(key) + " NOT NULL");
                }
                else
                    sb.Append(GetDefinition(key) + " NOT NULL IDENTITY(1,1)");

                sb.Append(',');
            }

            foreach (DbFieldInfo field in info.Fields)
            {
                sb.Append(GetDefinition(field));
                sb.Append(',');
            }

            string[] keys = getPrimaryKeys(info);
            sb.AppendFormat("PRIMARY KEY  ({0})", String.Join(",", keys));

            DbIndexesInfo indexes = DbAttributesManager.GetIndexes(info.Fields);
            ProcessIndexes(sb, indexes.Unique, ",UNIQUE ({0})");

            //TODO: (MSSQL) CREATE FULLTEXT INDEX 
            //TODO: (MSSQL) CREATE INDEX 
//            ProcessIndexes(sb, indexes.Indexes, ",KEY {1} ({0})");
//            ProcessIndexes(sb, indexes.FullText, ",FULLTEXT KEY {1} ({0})");

            //process foreign keys
            foreach (KeyValuePair<Type, DbFieldInfo> key in info.ForeignKeys)
            {
                DbIdentityRecordInfo ri = DbAttributesManager.GetRecordInfo(key.Key) as DbIdentityRecordInfo;
                if (ri == null)
                    throw new NdbException("Only DbIdentityRecord objects can be used as Foreign Keys");

                sb.AppendFormat(
                    ",FOREIGN KEY ([{1}]) REFERENCES [{0}] ([{2}]) ON DELETE CASCADE ON UPDATE CASCADE"
                    , ri.TableName
                    , key.Value.Name
                    , ri.PrimaryKey.Name);
            }

            sb.Append(")");

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

        internal override string[] LoadTables(DbGateway gateway)
        {
            return gateway.LoadArray<string>(
//                    @"SELECT TABLE_SCHEMA, TABLE_NAME, OBJECTPROPERTY(object_id(TABLE_NAME), N'IsUserTable') AS type 
                    @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE OBJECTPROPERTY(object_id(TABLE_NAME), N'IsUserTable')=1",
                    "TABLE_NAME");
        }

        public override string BuildLimits(string query, int limit, int offset)
        {
            if (offset == 0)
                return query.Insert(6, string.Concat(" TOP ", limit));

            int index = DbString.IndexOf(query, "ORDER BY");

            string select = (index >= 0) ? query.Substring(0, index) : query;
            string fields = select.Substring(7, select.IndexOf("FROM", StringComparison.OrdinalIgnoreCase) - 7);
            string orderby = (index >= 0) ? query.Substring(index) : "ORDER BY Id";

            select = select.Insert(6, " ROW_NUMBER() OVER(" + orderby + ") AS RowNum,");

            return string.Format(
                @"WITH Buffer AS ({0}) SELECT {4} FROM Buffer WHERE RowNum BETWEEN {3} AND {3}+{2}-1 {1};",
                select, orderby, limit, offset + 1, fields);
        }

    }
}