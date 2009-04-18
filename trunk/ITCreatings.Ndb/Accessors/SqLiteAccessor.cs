using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
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

        protected override System.Data.Common.DbDataAdapter GetAdapter()
        {
            return new SQLiteDataAdapter();
        }

        internal override string GetIdentity(string pk)
        {
            return ";SELECT LAST_INSERT_ROWID()";
        }

        internal override Dictionary<string, string> LoadFields(Type type)
        {
            string table = DbAttributesManager.GetTableName(type);
            try
            {
                object scalar = ExecuteScalar(string.Format("SELECT sql FROM sqlite_master WHERE type='table' and name='{0}'", table));
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
                throw new NdbException("Can't load fields information from the following table: " + table, ex);
            }
        }

        internal override string GetSqlType(Type type)
        {
            if (type == typeof(Byte))
                return "INTEGER";

            if (type == typeof(Int32))
                return "INTEGER";

            if (type == typeof(Int64))
                return "INTEGER";

            if (type == typeof(Int16))
                return "INTEGER";

            if (type == typeof(UInt32))
                return "INTEGER";

            if (type == typeof(UInt64))
                return "INTEGER";

            if (type == typeof(UInt16))
                return "INTEGER";

            if (type == typeof(string))
                return "VARCHAR";

            if (type.BaseType == typeof(Enum))
            {
                Type _type = Enum.GetUnderlyingType(type);
                return GetSqlType(_type);
            }

            if (type == typeof(DateTime))
                return "datetime";

            throw new NdbException("can't find SqLite type for the .NET Type" + type);
        }

        public override bool DropTable(string TableName)
        {
            try
            {
                ExecuteNonQuery("DROP TABLE " + TableName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(
                                      "Can't delete table {0} error {1}", TableName, ex.Message));
                return false;
            }
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
                sb.Append(getDefinition((info as DbIdentityRecordInfo).PrimaryKey) + " NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE");
                sb.Append(',');
            }

            foreach (FieldInfo field in info.Fields)
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

        private void createTriggers(DbRecordInfo info)
        {
            foreach (KeyValuePair<Type, FieldInfo> key in info.ForeignKeys)
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
    }
}