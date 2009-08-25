namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Internal class
    /// </summary>
    internal class DbIdentityRecordInfo : DbRecordInfo
    {
        public DbFieldInfo PrimaryKey;

        public bool IsDbGeneratedPrimaryKey { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="isDbGeneratedPrimaryKey">if set to <c>true</c> marks field as db generated.</param>
        public DbIdentityRecordInfo(DbFieldInfo primaryKey, bool isDbGeneratedPrimaryKey)
        {
            PrimaryKey = primaryKey;
            IsDbGeneratedPrimaryKey = isDbGeneratedPrimaryKey;
        }

        public bool IsPrimaryKeyValid(object data)
        {
            var primaryKey = PrimaryKey.GetValue(data);

            return !IsNull(primaryKey);
       }
    }
}