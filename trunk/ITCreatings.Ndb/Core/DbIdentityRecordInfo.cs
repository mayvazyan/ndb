using System;
using System.Reflection;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Internal class
    /// </summary>
    internal class DbIdentityRecordInfo : DbRecordInfo
    {
        public DbFieldInfo PrimaryKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="primaryKey"></param>
        public DbIdentityRecordInfo(DbFieldInfo primaryKey)
        {
            PrimaryKey = primaryKey;
        }

        public bool IsPrimaryKeyValid(object data)
        {
            var primaryKey = PrimaryKey.GetValue(data);

            return !IsNull(primaryKey);
       }
    }
}