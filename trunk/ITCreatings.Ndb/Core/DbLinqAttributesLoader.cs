#if LINQ
using System;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace ITCreatings.Ndb.Core
{
    internal class DbLinqAttributesLoader : DbAttributesLoader
    {
        public DbLinqAttributesLoader(Type type, TableAttribute tableAttribute)
        {
            DbFieldInfo primaryKey = null;

            MemberInfo[] fields = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MemberInfo field in fields)
            {
                object[] attributes = field.GetCustomAttributes(typeof (ColumnAttribute), true);
                if (attributes.Length > 0)
                {
                    ColumnAttribute attribute = (ColumnAttribute) attributes[0];

                    DbFieldInfo dbFieldInfo = new DbFieldInfo(field, attribute.Name, 0, null, null);

                    if (attribute.IsPrimaryKey)
                    {
                        primaryKey = dbFieldInfo;
                    }
                    else
                    {
                        dbFields.Add(dbFieldInfo);
                    }
                }
            }

            RecordInfo = (primaryKey != null)
                             ? new DbIdentityRecordInfo(primaryKey, false)
                             : new DbRecordInfo();

            RecordInfo.RecordType = type;
//            RecordInfo.ForeignKeys = foreignKeys;
            RecordInfo.TableName = tableAttribute.Name;
            RecordInfo.Fields = dbFields.ToArray();
//            RecordInfo.Childs = childs;
//            RecordInfo.Parents = parents;
        }
    }
}
#endif