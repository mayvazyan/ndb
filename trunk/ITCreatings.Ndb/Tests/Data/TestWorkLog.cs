#if DEBUG
using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Keys;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("WorkLogs")]
    public class TestWorkLog : DbActiveRecord
    {
        [DbPrimaryKeyField] public ulong Id;
        [DbField] public uint TaskId;
        [DbForeignKeyField(typeof(TestUser))] public ulong UserId;
        [DbField] public uint SpentMinutes;
        [DbField] public DateTime Date;
        [DbField] public string Description;
        [DbField] public DateTime Timestamp;
    }
}
#endif