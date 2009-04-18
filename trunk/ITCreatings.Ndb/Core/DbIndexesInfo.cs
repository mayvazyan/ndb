using System;
using System.Collections.Generic;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Contains info about type's indexies
    /// </summary>
    internal class DbIndexesInfo
    {
        public Dictionary<string, List<string>> Unique = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Indexes = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> FullText = new Dictionary<string, List<string>>();

        public void AddUnique(string indexName, string name)
        {
            Add(Unique, indexName, name);
        }

        public void AddIndexes(string indexName, string name)
        {
            Add(Indexes, indexName, name);
        }

        public void AddFullText(string indexName, string name)
        {
            Add(FullText, indexName, name);
        }

        public static void Add(Dictionary<string, List<string>> dict, string indexName, string name)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                dict.Add("ndb_" + dict.Count + name, new List<string> { name });
                return;
            }

            if (!dict.ContainsKey(indexName))
                dict.Add(indexName, new List<string> { name });
            else
                dict[indexName].Add(name);
        }
    }
}