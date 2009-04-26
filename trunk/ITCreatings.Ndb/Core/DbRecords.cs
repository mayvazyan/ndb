using System;

namespace ITCreatings.Ndb.Core
{
    internal class DbRecords
    {
        public DbRecordInfo RecordInfo { get; private set; }
        public Array List { get; private set; }

        public DbRecords(DbRecordInfo recordInfo, Array list)
        {
            RecordInfo = recordInfo;
            List = list;
        }
    }
}
