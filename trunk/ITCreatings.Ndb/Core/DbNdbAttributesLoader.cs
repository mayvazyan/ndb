using System;
using System.Collections.Generic;
using System.Reflection;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Attributes Loader helper
    /// </summary>
    internal class DbNdbAttributesLoader : DbAttributesLoader
    {
        readonly DbFieldInfo primaryKey;
        readonly Dictionary<Type, FieldInfo> childs = new Dictionary<Type, FieldInfo>();
        readonly Dictionary<Type, FieldInfo> parents = new Dictionary<Type, FieldInfo>();
        readonly Dictionary<Type, DbFieldInfo> foreignKeys = new Dictionary<Type, DbFieldInfo>();

        public DbNdbAttributesLoader(Type type)
        {
            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                object[] parentRecordsAttributes = field.GetCustomAttributes(typeof (DbParentRecordAttribute), true);
                if (parentRecordsAttributes != null && parentRecordsAttributes.Length > 0)
                {
                    parents.Add(field.FieldType, field);
                }

                object[] childRecordsAttributes = field.GetCustomAttributes(typeof (DbChildRecordsAttribute), true);
                if (childRecordsAttributes != null && childRecordsAttributes.Length > 0)
                {
                    if (field.FieldType.BaseType != typeof (Array))
                        throw new NdbException("DbChildRecordsAttribute can belong to Array field ONLY");

                    childs.Add(field.FieldType.GetElementType(), field);
                }

                object[] attributes = field.GetCustomAttributes(typeof (DbFieldAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    var foreignTypes = new List<Type>(attributes.Length);
                    bool isPrimary = false;
                    object defaultValue = null;
                    string Name = null;
                    uint Size = 0;
                    Type DbType = null;
                    foreach (DbFieldAttribute attribute in attributes)
                    {
                        if (attribute.DefaultValue != null)
                            defaultValue = attribute.DefaultValue;

                        if (attribute.DiffersFromDatabaseType)
                            DbType = attribute.DbType;

                        if (attribute.Size > 0)
                            Size = attribute.Size;

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
                    DbFieldInfo dbFieldInfo = new DbFieldInfo(field, Name, Size, DbType, defaultValue);

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

            RecordInfo = (primaryKey != null)
                             ? new DbIdentityRecordInfo(primaryKey)
                             : new DbRecordInfo();

            RecordInfo.RecordType = type;
            RecordInfo.ForeignKeys = foreignKeys;
            RecordInfo.TableName = LoadTableName(type);
            RecordInfo.Fields = dbFields.ToArray();
            RecordInfo.Childs = childs;
            RecordInfo.Parents = parents;
        }

        internal static string LoadTableName(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(DbRecordAttribute), false);
            DbRecordAttribute recordAttribute = null;

            if (attributes.Length > 0)
                recordAttribute = attributes[0] as DbRecordAttribute;

            if (recordAttribute == null || string.IsNullOrEmpty(recordAttribute.TableName))
                return type.Name + "s";

            return recordAttribute.TableName;
        }

    }
}