﻿using System;
using System.Collections.Generic;
using System.Reflection;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Indexes;
using ITCreatings.Ndb.Attributes.Keys;

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
                FieldInfo primaryKey = null;
                List<FieldInfo> dbFields = new List<FieldInfo>();
                Dictionary<Type, FieldInfo> foreignKeys = new Dictionary<Type, FieldInfo>();

                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo field in fields)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DbFieldAttribute), true);
                    if (attributes.Length > 0)
                    {
                        foreach (DbFieldAttribute attribute in attributes)
                        {
//                            DbFieldAttribute attribute = (DbFieldAttribute)attributes[0];// as DbPrimaryKeyFieldAttribute;
                            if (attribute is DbPrimaryKeyFieldAttribute)
                                primaryKey = field;
                            else
                            {
                                var dbForeignKeyFieldAttribute = attribute as DbForeignKeyFieldAttribute;
                                if (dbForeignKeyFieldAttribute != null)
                                    foreignKeys.Add(dbForeignKeyFieldAttribute.Type, field);
                            }
                        }
                        if (primaryKey != field)
                            dbFields.Add(field);
                    }
                }

                DbRecordInfo info = (primaryKey != null) ? new DbIdentityRecordInfo(primaryKey) : new DbRecordInfo();

                info.RecordType = type;
                info.ForeignKeys = foreignKeys;
                info.TableName = LoadTableName(type);
                info.Fields = dbFields.ToArray();
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


//        public static Type[] LoadDbRecordTypes()
//        {
//            return LoadDbRecordTypes(Assembly);
//        }

        
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

        public static DbIndexesInfo GetIndexes(FieldInfo[] fields)
        {
            var indexesInfo = new DbIndexesInfo();

            foreach (var field in fields)
            {
                object[] attributes = field.GetCustomAttributes(true);
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