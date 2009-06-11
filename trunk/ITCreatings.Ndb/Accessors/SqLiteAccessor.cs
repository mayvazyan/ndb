using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Accessors
{
    internal class SqLiteAccessor : DbAccessor
    {
        protected override System.Data.Common.DbCommand Command(string query)
        {
            return new SQLiteCommand(query, new SQLiteConnection(ConnectionString));
        }

        public override System.Data.Common.DbDataAdapter GetAdapter()
        {
            return new SQLiteDataAdapter();
        }

        internal override string GetIdentity(string pk)
        {
            return ";SELECT LAST_INSERT_ROWID()";
        }

        internal override Dictionary<string, string> LoadFields(DbGateway gateway, string tableName)
        {
            try
            {
                object scalar = ExecuteScalar(string.Format("SELECT sql FROM sqlite_master WHERE type='table' and name='{0}'", tableName));
                string sql = scalar.ToString();
                int start = sql.IndexOf('(') + 1;
                sql = sql.Substring(start, sql.IndexOf(')', start) - start);
                string[] args = sql.Split(new[] {','});

                Dictionary<string, string> d = new Dictionary<string, string>();
                foreach (var item in args)
                {
                    string[] split = item.Split(' ');
                    string key = split[0];
                    string value = split[1];

                    d.Add(key, value);
                }
                return d;
            }
            catch (Exception ex)
            {
                throw new NdbException("Can't load fields information from the following table: " + tableName, ex);
            }
        }

        internal override string GetSqlType(Type type, uint size)
        {
            if (type == typeof(Byte)) return "INTEGER";

            if (type == typeof(Int16)) return "INTEGER"; 
            if (type == typeof(Int32)) return "INTEGER";
            if (type == typeof(Int64)) return "INTEGER";

            if (type == typeof(UInt16)) return "INTEGER";
            if (type == typeof(UInt32)) return "INTEGER";
            if (type == typeof(UInt64)) return "INTEGER";

            if (type == typeof(string)) return "VARCHAR";

            if (type == typeof(Guid)) return "BLOB";
            if (type == typeof(DateTime) || type == typeof(DateTime?)) return "TIMESTAMP";

            if (type == typeof(Double)) return "FLOAT";
            if (type == typeof(Decimal)) return "FLOAT";
            if (type == typeof(Boolean)) return "BOOLEAN";
            if (type == typeof(Byte[])) return "BLOB";

            throw new NdbUnsupportedColumnTypeException(Provider, type);
        }

        public override void DropTable(string tableName)
        {
            ExecuteNonQuery("DROP TABLE IF EXISTS " + tableName);
        }

        internal override void AlterTable(DbTableCheckResult checkResult)
        {
            throw new NotImplementedException("besides poor ALTER TABLE support by SqLite, Ndb can't update it's tables now");
        }

        internal override void CreateTable(DbRecordInfo info)
        {
            StringBuilder sb = new StringBuilder("CREATE TABLE " + info.TableName + "(");
            if (info is DbIdentityRecordInfo)
            {
                DbFieldInfo key = (info as DbIdentityRecordInfo).PrimaryKey;

                if (key.FieldType == typeof(Guid))
                    sb.Append(getDefinition(key) + " NOT NULL PRIMARY KEY UNIQUE");
                else
                    sb.Append(getDefinition(key) + " NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE");

                sb.Append(',');
            }

            foreach (DbFieldInfo field in info.Fields)
            {
                sb.Append(getDefinition(field));
                sb.Append(',');
            }

            sb.Remove(sb.Length - 1, 1);

            sb.Append(")");

            string query = sb.ToString();
            try
            {
                ExecuteNonQuery(query);
                createTriggers(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(
                                      "query {0} failed this error {1}", query, ex.Message));

                throw;
            }
        }

        internal override string[] LoadTables(DbGateway gateway)
        {
            return gateway.LoadArray<string>(
                @"SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", "name");
        }

        private void createTriggers(DbRecordInfo info)
        {
            foreach (KeyValuePair<Type, DbFieldInfo> key in info.ForeignKeys)
            {
                DbIdentityRecordInfo keyInfo = DbAttributesManager.GetRecordInfo(key.Key) as DbIdentityRecordInfo;
                if (keyInfo == null)
                    throw new NdbException("Only Identity records supported");

                string TriggerBase = string.Format(@"_{0}_{1}_{2}_{3}"
                                                   , info.TableName, key.Value.Name
                                                   , keyInfo.TableName, keyInfo.PrimaryKey.Name);

                string TriggerName = "fki" + TriggerBase;
                dropTrigger(TriggerName);

                ExecuteNonQuery(string.Format(@"CREATE TRIGGER {0}
                    BEFORE INSERT ON [{1}]
                    FOR EACH ROW BEGIN
                      SELECT RAISE(ROLLBACK, 'insert on table ""{1}"" violates foreign key constraint ""{0}""')
                      WHERE NEW.{2} IS NOT NULL AND (SELECT {3} FROM {4} WHERE {3} = NEW.{2}) IS NULL;
                    END;"
                                              , TriggerName
                                              , info.TableName
                                              , key.Value.Name
                                              , keyInfo.PrimaryKey.Name
                                              , keyInfo.TableName
                                    ));

                TriggerName = "fku" + TriggerBase;
                dropTrigger(TriggerName);
                

                ExecuteNonQuery(string.Format(@"CREATE TRIGGER {0}
                    BEFORE UPDATE ON [{1}]
                    FOR EACH ROW BEGIN
                        SELECT RAISE(ROLLBACK, 'update on table ""{1}"" violates foreign key constraint ""{0}""')
                          WHERE NEW.{2} IS NOT NULL AND (SELECT {3} FROM {4} WHERE {3} = NEW.{2}) IS NULL;
                    END;"
                                              , TriggerName
                                              , info.TableName
                                              , key.Value.Name
                                              , keyInfo.PrimaryKey.Name
                                              , keyInfo.TableName
                                    ));

                TriggerName = "fkdc" + TriggerBase;
                dropTrigger(TriggerName);

                ExecuteNonQuery(string.Format(@"CREATE TRIGGER {0}
                      BEFORE DELETE ON {1}
                      FOR EACH ROW BEGIN
                          DELETE FROM {2} WHERE {3} = OLD.{4};
                    END;"
                                              , TriggerName
                                              , keyInfo.TableName
                                              , info.TableName
                                              , key.Value.Name
                                              , keyInfo.PrimaryKey.Name
                                    ));
            }
/*        
                -- Drop Trigger
                DROP TRIGGER fki_bar_fooId_foo_id;

                -- Foreign Key Preventing insert
                CREATE TRIGGER fki_bar_fooId_foo_id
                BEFORE INSERT ON [bar]
                FOR EACH ROW BEGIN
                  SELECT RAISE(ROLLBACK, 'insert on table "bar" violates foreign key constraint "fki_bar_fooId_foo_id"')
                  WHERE NEW.fooId IS NOT NULL AND (SELECT id FROM foo WHERE id = NEW.fooId) IS NULL;
                END;

                -- Drop Trigger
                DROP TRIGGER fku_bar_fooId_foo_id;

                -- Foreign key preventing update
                CREATE TRIGGER fku_bar_fooId_foo_id
                BEFORE UPDATE ON [bar]
                FOR EACH ROW BEGIN
                    SELECT RAISE(ROLLBACK, 'update on table "bar" violates foreign key constraint "fku_bar_fooId_foo_id"')
                      WHERE NEW.fooId IS NOT NULL AND (SELECT id FROM foo WHERE id = NEW.fooId) IS NULL;
                END;

                -- Drop Trigger
                DROP TRIGGER fkd_bar_fooId_foo_id;

                -- Foreign key preventing delete
                CREATE TRIGGER fkd_bar_fooId_foo_id
                BEFORE DELETE ON foo
                FOR EACH ROW BEGIN
                  SELECT RAISE(ROLLBACK, 'delete on table "foo" violates foreign key constraint "fkd_bar_fooId_foo_id"')
                  WHERE (SELECT fooId FROM bar WHERE fooId = OLD.id) IS NOT NULL;
                END;

                -- Drop Trigger
                DROP TRIGGER fki_bar_fooId2_foo_id;

                -- Foreign Key Preventing insert
                CREATE TRIGGER fki_bar_fooId2_foo_id
                BEFORE INSERT ON [bar]
                FOR EACH ROW BEGIN
                  SELECT RAISE(ROLLBACK, 'insert on table "bar" violates foreign key constraint "fki_bar_fooId2_foo_id"')
                  WHERE (SELECT id FROM foo WHERE id = NEW.fooId2) IS NULL;
                END;

                -- Drop Trigger
                DROP TRIGGER fku_bar_fooId2_foo_id;

                -- Foreign key preventing update
                CREATE TRIGGER fku_bar_fooId2_foo_id
                BEFORE UPDATE ON [bar]
                FOR EACH ROW BEGIN
                    SELECT RAISE(ROLLBACK, 'update on table "bar" violates foreign key constraint "fku_bar_fooId2_foo_id"')
                      WHERE (SELECT id FROM foo WHERE id = NEW.fooId2) IS NULL;
                END;

                -- Drop Trigger
                DROP TRIGGER fkdc_bar_fooId2_foo_id;

                -- Cascading Delete
                CREATE TRIGGER fkdc_bar_fooId2_foo_id
                BEFORE DELETE ON foo
                FOR EACH ROW BEGIN
                    DELETE FROM bar WHERE bar.fooId2 = OLD.id;
                END;
*/
        }

        private void dropTrigger(string triggerName)
        {
            ExecuteNonQuery("DROP TRIGGER IF EXISTS " + triggerName);
        }

        public override string BuildLimits(string query, int limit, int offset)
        {
            return string.Concat(query, " LIMIT ", offset.ToString(), ",", limit.ToString());
        }
    }
}