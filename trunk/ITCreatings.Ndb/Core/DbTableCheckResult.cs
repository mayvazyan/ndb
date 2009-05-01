using System;
using System.Collections.Generic;
using System.Reflection;

namespace ITCreatings.Ndb.Core
{
    internal class DbTableCheckResult
    {
        private readonly DbAccessor accessor;
        
        public string LastError { get; private set; }
        public string TableName;
        public IDictionary<string, string> FieldsToCreate { get; private set; }
        public IDictionary<string, string> FieldsToUpdate { get; private set; }

        public DbTableCheckResult(DbAccessor accessor)
        {
            this.accessor = accessor;
        }

        public void Build(Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            
            TableName = info.TableName;

            Dictionary<string, string> fields = accessor.LoadFields(TableName);

            FieldsToCreate = new Dictionary<string, string>();
            FieldsToUpdate = new Dictionary<string, string>();

            DbFieldCheckResult checker = new DbFieldCheckResult(accessor);
            foreach (DbFieldInfo fi in info.Fields)
            {
                checker.Process(fields, fi);

                if (checker.IsNew)
                    FieldsToCreate.Add(fi.Name, checker.SqlType);
                else
                    if (checker.IsDifferent)
                        FieldsToUpdate.Add(fi.Name, checker.SqlType);
            }
        }

        public bool IsAllFieldValid(Type type)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            Dictionary<string, string> fields = accessor.LoadFields(info.TableName);

            DbFieldCheckResult checker = new DbFieldCheckResult(accessor);

            foreach (DbFieldInfo fi in info.Fields)
            {
                checker.Process(fields, fi);

                if (checker.IsNew)
                {
                    LastError = string.Format("{0} in {1} ({2}) isn't present in db"
                                          , fi.Name, type, fi.FieldType);
                    return false;
                }

                if (checker.IsDifferent)
                {       
                    LastError = string.Format("{0} in {1} is {2} but column is {3}"
                                                  , fi.Name, type, fi.FieldType, checker.CurrentSqlType);
                    return false;
                }
            }

            return true;
        }
/*
        private bool IsFieldValid(IDictionary<string, string> fields, Type recordType, Type fieldType, string fieldName)
        {
            if (!fields.ContainsKey(fieldName))
            {
                LastError = string.Format("{0} in {1} ({2}) isn't present in db"
                                          , fieldName, recordType, fieldType);
                return false;
            }

            if (!accessor.IsPostgre)
            {
                Type sqlType = accessor.GetType(fields[fieldName]);

                if (fieldType.BaseType == typeof(Enum))
                    fieldType = Enum.GetUnderlyingType(fieldType);

                if (sqlType != fieldType)
                {
                    LastError = string.Format("{0} in {1} is {2} but column is {3}"
                                              , fieldName, recordType, fieldType, sqlType);
                    return false;
                }
            }
            else
            {
                string sqlType = accessor.GetSqlType(fieldType);
                string curSqlType = fields[fieldName];
                if (sqlType != curSqlType)
                {
                    LastError = string.Format("{0} in {1} is {2} but column is {3}"
                                              , fieldName, recordType, sqlType, curSqlType);
                    return false;
                }
            }
            return true;
        }
*/

    }
}
