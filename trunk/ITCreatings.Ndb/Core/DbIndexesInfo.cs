using System.Collections.Generic;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Contains info about type's indexies
    /// </summary>
    public class DbIndexesInfo
    {
        /// <summary>
        /// Unique indexes
        /// </summary>
        public Dictionary<string, List<string>> Unique = new Dictionary<string, List<string>>();
        
        /// <summary>
        /// Indexes
        /// </summary>
        public Dictionary<string, List<string>> Indexes = new Dictionary<string, List<string>>();

        /// <summary>
        /// Full text indexes
        /// </summary>
        public Dictionary<string, List<string>> FullText = new Dictionary<string, List<string>>();


        /// <summary>
        /// Adds the unique index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="name">The name.</param>
        public void AddUnique(string indexName, string name)
        {
            Add(Unique, indexName, name);
        }

        /// <summary>
        /// Adds the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="name">The name.</param>
        public void AddIndex(string indexName, string name)
        {
            Add(Indexes, indexName, name);
        }

        /// <summary>
        /// Adds the full text index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="name">The name.</param>
        public void AddFullText(string indexName, string name)
        {
            Add(FullText, indexName, name);
        }

        private static void Add(Dictionary<string, List<string>> dict, string indexName, string name)
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