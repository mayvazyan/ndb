using System.Collections.Generic;

namespace ITCreatings.Ndb.Core
{
    internal class DbAttributesLoader
    {
        protected readonly List<DbFieldInfo> dbFields = new List<DbFieldInfo>();
        public DbRecordInfo RecordInfo { get; protected set; }
    }
}