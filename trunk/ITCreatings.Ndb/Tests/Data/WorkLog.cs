#if DEBUG
using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("WorkLogs")]
    public class WorkLog : DbActiveRecord
    {
        [DbPrimaryKeyField] public long Id;
        [DbField] public int TaskId;
        [DbForeignKeyField(typeof(User))] public long UserId;
        [DbField] public int SpentMinutes;
        [DbField] public DateTime? Date;
        [DbField] public string Description;
        [DbField] public DateTime? Timestamp;
    }
}
#endif