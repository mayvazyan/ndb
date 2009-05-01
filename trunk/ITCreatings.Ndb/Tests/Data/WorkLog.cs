#if DEBUG
using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("WorkLogs")]
    public class WorkLog : DbActiveRecord
    {
        [DbPrimaryKeyField] public ulong Id;
        [DbField] public uint TaskId;
        [DbForeignKeyField(typeof(User))] public ulong UserId;
        [DbField] public uint SpentMinutes;
        [DbField] public DateTime Date;
        [DbField] public string Description;
        [DbField] public DateTime Timestamp;
    }
}
#endif