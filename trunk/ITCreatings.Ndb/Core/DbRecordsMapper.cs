using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Core
{
    internal static class DbRecordsMapper
    {
        #region Static

        public static void Map(object[] parents, object[] childs)
        {
            if (IsEmpty(parents) || IsEmpty(childs))
                return;

            DbRecordInfo childRecordInfo = DbAttributesManager.GetRecordInfo(childs[0].GetType());
            Type parentType = parents[0].GetType();
            DbIdentityRecordInfo primaryRecordInfo = DbAttributesManager.GetRecordInfo(parentType) as DbIdentityRecordInfo;

            if (primaryRecordInfo == null)
                throw new NdbNotIdentityException("Only DbIdentityRecord objects can have childs");

            if (!primaryRecordInfo.Childs.ContainsKey(childRecordInfo.RecordType))
                throw new NdbRelationException("{0} doesn't contains Childs of {1} type",
                    primaryRecordInfo.RecordType, childRecordInfo.RecordType);

            MemberInfo childReferenceField = primaryRecordInfo.Childs[childRecordInfo.RecordType];
            Type childType = DbFieldInfo.GetType(childReferenceField).GetElementType();

            var childsList = new List<object>(childs);
            foreach (object parent in parents)
            {
                ArrayList list = new ArrayList();
                object pkvalue = primaryRecordInfo.PrimaryKey.GetValue(parent);
                for (int i = childsList.Count - 1; i >= 0; i--)
                {
                    object childRecord = childsList[i];
                    object foreignKeyValue = childRecordInfo.ForeignKeys[parentType].GetValue(childRecord);
                    
                    if (pkvalue.Equals(foreignKeyValue))
                    {
                        Type type = parent.GetType();
                        if (childRecordInfo.Parents.ContainsKey(type))
                        {
                            DbFieldInfo.SetValue(childRecordInfo.Parents[type], childRecord, parent);
                        }

                        list.Add(childRecord);
                        childsList.RemoveAt(i);
                    }
                }

                DbFieldInfo.SetValue(childReferenceField, parent, list.ToArray(childType));
            }
        }

        public static bool IsEmpty(object[] list)
        {
            return (list == null || list.Length == 0);
        }

        #endregion
    }
}
