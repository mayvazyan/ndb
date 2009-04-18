using System;
using System.Reflection;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Internal class
    /// </summary>
    internal class DbIdentityRecordInfo : DbRecordInfo
    {
        public FieldInfo PrimaryKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="primaryKey"></param>
        public DbIdentityRecordInfo(FieldInfo primaryKey)
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