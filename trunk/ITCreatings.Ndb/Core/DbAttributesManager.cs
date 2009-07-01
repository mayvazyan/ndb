using System;
using System.Collections.Generic;
using System.Reflection;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Provide access to DbAttributes
    /// </summary>
    public class DbAttributesManager
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
        
        /// <summary>
        /// Load Record Structure Information
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbRecordInfo GetRecordInfo(Type type)
        {
            lock (records)
            {
                if (!records.ContainsKey(type))
                {
                    DbRecordInfo recordInfo = LoadRecordInfo(type);

                    records.Add(type, recordInfo);
                }
            }
            return records[type];
        }

        private static DbRecordInfo LoadRecordInfo(Type type)
        {
#if LINQ
            object[] attributes = type.GetCustomAttributes(true);
            foreach (object attribute in attributes)
            {
                if (attribute is DbRecordAttribute)
                {
                    break;
                }
                if (attribute is System.Data.Linq.Mapping.TableAttribute)
                {
                    DbLinqAttributesLoader loader = new DbLinqAttributesLoader(type, (System.Data.Linq.Mapping.TableAttribute)attribute);
                    return loader.RecordInfo;
                }
            }
#endif
            DbNdbAttributesLoader attributesManager = new DbNdbAttributesLoader(type);
            return attributesManager.RecordInfo;
        }

        /// <summary>
        /// Gets the records info.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static DbRecordInfo[] GetRecordInfo(Type[] type)
        {
            var list = new DbRecordInfo[type.Length];
            for (int i = 0; i < type.Length; i++)
            {
                list[i] = GetRecordInfo(type[i]);
            }
            return list;
        }

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

        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
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
                        indexesInfo.AddIndex(attribute.IndexName, field.Name);
                }
            }

            return indexesInfo;
        }
    }
}