using System;
using System.Collections.Generic;
using System.Reflection;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Indexes;
using ITCreatings.Ndb.Attributes.Keys;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Provide access to DbAttributes
    /// </summary>
    internal class DbAttributesManager
    {
        private static readonly Dictionary<Type, DbRecordInfo> records = new Dictionary<Type, DbRecordInfo>();

        /// <summary>
        /// Returns Table Name assosiated with the specifyed type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            return GetRecordInfo(type).TableName;
        }

        private static string LoadTableName(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(DbRecordAttribute), false);
            DbRecordAttribute recordAttribute = null;

            if (attributes.Length > 0)
                recordAttribute = attributes[0] as DbRecordAttribute;

            if (recordAttribute == null || string.IsNullOrEmpty(recordAttribute.TableName))
                return type.Name + "s";

            return recordAttribute.TableName;
        }

        /// <summary>
        /// Load Record Structure Information
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbRecordInfo GetRecordInfo(Type type)
        {
            if (!records.ContainsKey(type))
            {
                DbFieldInfo primaryKey = null;
                List<DbFieldInfo> dbFields = new List<DbFieldInfo>();
                Dictionary<Type, FieldInfo> childs = new Dictionary<Type, FieldInfo>();
                Dictionary<Type, FieldInfo> parents = new Dictionary<Type, FieldInfo>();
                Dictionary<Type, DbFieldInfo> foreignKeys = new Dictionary<Type, DbFieldInfo>();

                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo field in fields)
                {
                    object[] parentRecordsAttributes = field.GetCustomAttributes(typeof(DbParentRecordAttribute), true);
                    if (parentRecordsAttributes != null && parentRecordsAttributes.Length > 0)
                    {
                        parents.Add(field.FieldType, field);
                    }

                    object[] childRecordsAttributes = field.GetCustomAttributes(typeof(DbChildRecordsAttribute), true);
                    if (childRecordsAttributes != null && childRecordsAttributes.Length > 0)
                    {
                        if (field.FieldType.BaseType != typeof(Array))
                            throw new NdbException("DbChildRecordsAttribute can belong to Array field ONLY");

                        childs.Add(field.FieldType.GetElementType(), field);
                    }

                    object[] attributes = field.GetCustomAttributes(typeof(DbFieldAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        List<Type> foreignTypes = new List<Type>(attributes.Length);
                        bool isPrimary = false;
                        string Name = null;
                        foreach (DbFieldAttribute attribute in attributes)
                        {
                            if (string.IsNullOrEmpty(Name))
                                Name = attribute.Name;

                            if (attribute is DbPrimaryKeyFieldAttribute)
                            {
                                isPrimary = true;
                            }
                            else
                            {
                                var dbForeignKeyFieldAttribute = attribute as DbForeignKeyFieldAttribute;
                                if (dbForeignKeyFieldAttribute != null)
                                    foreignTypes.Add(dbForeignKeyFieldAttribute.Type);
//                                    foreignKeys.Add(dbForeignKeyFieldAttribute.Type, dbFieldInfo);
                            }
                        }
                        DbFieldInfo dbFieldInfo = new DbFieldInfo(field, Name);

                        if (isPrimary)
                        {
                            primaryKey = dbFieldInfo;
                        }
                        else
                        {
                            dbFields.Add(dbFieldInfo);
                        }

                        foreach (Type foreignType in foreignTypes)
                        {
                            foreignKeys.Add(foreignType, dbFieldInfo);
                        }
                    }
                }

                DbRecordInfo info = (primaryKey != null) ? new DbIdentityRecordInfo(primaryKey) : new DbRecordInfo();

                info.RecordType = type;
                info.ForeignKeys = foreignKeys;
                info.TableName = LoadTableName(type);
                info.Fields = dbFields.ToArray();
                info.Childs = childs;
                info.Parents = parents;
                records.Add(type, info);
            }

            return records[type];
        }
        /*
        public static DbViewInfo GetViewInfo(Type type)
        {
            if (!views.ContainsKey(type))
            {
                FieldInfo primaryKey = null;
                List<FieldInfo> dbFields = new List<FieldInfo>();
                Dictionary<Type, FieldInfo> foreignKeys = new Dictionary<Type, FieldInfo>();

                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo field in fields)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DbFieldAttribute), true);
                    if (attributes.Length > 0)
                    {
                        DbFieldAttribute attribute = (DbFieldAttribute)attributes[0];// as DbPrimaryKeyFieldAttribute;
                        if (attribute is DbPrimaryKeyFieldAttribute)
                            primaryKey = field;
                        else
                        {
                            var dbForeignKeyFieldAttribute = attribute as DbForeignKeyFieldAttribute;
                            if (dbForeignKeyFieldAttribute != null)
                                foreignKeys.Add(dbForeignKeyFieldAttribute.Type, field);

                            dbFields.Add(field);
                        }
                    }
                }

                var info = new DbViewInfo();
                views.Add(type, info);
            }

            return views[type];
        }*/

        /// <summary>
        /// Load all types with DbRecordAttribute
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Type[] LoadDbRecordTypes(Assembly assembly)
        {
            var list = new List<Type>();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(DbRecordAttribute), true);
                if (attributes.Length > 0)
                    list.Add(type);
            }

            return list.ToArray();
        }

        public static DbIndexesInfo GetIndexes(DbFieldInfo[] fields)
        {
            var indexesInfo = new DbIndexesInfo();

            foreach (var field in fields)
            {
                object[] attributes = field.GetCustomAttributes();
                foreach (var _attribute in attributes)
                {
                    var attribute = _attribute as DbIndexedFieldAttribute;

                    if (attribute == null)
                        continue;

                    if (attribute is DbUniqueFieldAttribute)
                        indexesInfo.AddUnique(attribute.IndexName, field.Name);
                    else if (attribute is DbFullTextIndexedFieldAttribute)
                        indexesInfo.AddFullText(attribute.IndexName, field.Name);
                    else
                        indexesInfo.AddIndexes(attribute.IndexName, field.Name);
                }
            }

            return indexesInfo;
        }
    }
}