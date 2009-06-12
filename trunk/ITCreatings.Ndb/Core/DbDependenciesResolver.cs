using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Core
{
    internal class DbDependenciesResolver<T>
    {
        public T[] Root { get { return (T[]) rootRecords.List;} }

        private DbRecords rootRecords;
        private Dictionary<Type, DbRecords> recordSets;
        private readonly Type[] dependenciesTypes;

        public DbDependenciesResolver(IDataReader dataReader, params Type[] types)
        {
            if (types.Length == 0)
                throw new NdbException("You should specify at least one Dependency Type");

            dependenciesTypes = types;
            loadRecordSets(dataReader);
            mapRecordSets();
        }

        private void mapRecordSets()
        {
            mapRecordSets(rootRecords);
            foreach (KeyValuePair<Type, DbRecords> recordSet in recordSets)
            {
                mapRecordSets(recordSet.Value);
            }
        }

        private void mapRecordSets(DbRecords dbRecords)
        {
            Dictionary<Type, MemberInfo> childs = dbRecords.RecordInfo.Childs;
            foreach (KeyValuePair<Type, MemberInfo> child in childs)
            {
                Type childRecordType = child.Key;
                
                if (recordSets.ContainsKey(childRecordType))
                {
                    DbRecords records = recordSets[childRecordType];
                    DbRecordsMapper.Map((object[])dbRecords.List, (object[])records.List);
                }
            }
        }

        private void loadRecordSets(IDataReader reader)
        {
            recordSets = new Dictionary<Type, DbRecords>();
            rootRecords = loadRecords(typeof(T), reader);
            int i = 0;
            while (reader.NextResult() && i < dependenciesTypes.Length)
            {
                Type type = dependenciesTypes[i++];
                DbRecords dbRecords = loadRecords(type, reader);
                recordSets.Add(type, dbRecords);
            } 
        }

        private static DbRecords loadRecords(Type type, IDataReader reader)
        {
            DbRecordInfo info = DbAttributesManager.GetRecordInfo(type);
            Array records = DbGateway.LoadRecords(type, reader, info);
            return new DbRecords(info, records);
        }
    }
}
