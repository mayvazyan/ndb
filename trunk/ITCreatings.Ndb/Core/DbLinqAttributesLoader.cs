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
            bool IsDbGeneratedPrimaryKey = false;
            DbFieldInfo primaryKey = null;
            bool primaryKeyFound = false;
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
                        if (primaryKeyFound) //found second primary key mean it's composite
                        {
                            if (primaryKey != null)
                            {
                                dbFields.Add(primaryKey);
                                primaryKey = null;
                            }
                            dbFields.Add(dbFieldInfo);
                        }
                        else
                        {
                            IsDbGeneratedPrimaryKey = attribute.IsDbGenerated;
                            primaryKey = dbFieldInfo;
                        }
                        primaryKeyFound = true;
                    }
                    else
                    {
                        dbFields.Add(dbFieldInfo);
                    }
                }
            }

            RecordInfo = (primaryKey != null)
                             ? new DbIdentityRecordInfo(primaryKey, IsDbGeneratedPrimaryKey)
                             : new DbRecordInfo();

            RecordInfo.RecordType = type;
//            RecordInfo.ForeignKeys = foreignKeys;
            RecordInfo.TableName = string.IsNullOrEmpty(tableAttribute.Name) ? type.Name :  tableAttribute.Name;
            RecordInfo.Fields = dbFields.ToArray();
//            RecordInfo.Childs = childs;
//            RecordInfo.Parents = parents;
        }
    }
}
#endif