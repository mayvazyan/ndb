#if DEBUG
using System;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    public enum EventType : short
    {
        // (means that the entry added to the Logs table)
        ActivityLog = 1,
        Logon = 101,
        Logoff = 102,
        IdleStart = 201, 
        IdleEnd = 202
    }

    [DbRecord("WorkLogs")]
    public class TestWorkLogItem
    {
        [DbPrimaryKeyField]
        public Int32 Id;

        [DbField]
        public Int32 TaskId;

        [DbForeignKeyField(typeof(User))]
        public Int64 UserId;

        [DbField]
        public int SpentMinutes;

        [DbField("CreationDate")]
        public DateTime Date;
        [DbField]
        public string Description;
        [DbField]
        public DateTime Timestamp;
    }

    [DbRecord("WorkLogs")]
    public class TestWorkLogItem3
    {
        [DbPrimaryKeyField]
        public Int32 Id;

        [DbField]
        public string Description2;
    }

    [DbRecord("WorkLogs")]
    public class TestWorkLogItem2
    {
        [DbPrimaryKeyField]
        public Int32 Id;

        [DbField]
        public Int32 TaskId;

        [DbForeignKeyField(typeof(User))]
        public Int16 UserId;

        [DbField]
        public uint SpentMinutes;
        [DbField]
        public DateTime Date;
        [DbField]
        public string Description;
        [DbField]
        public DateTime Timestamp;
    }
}
#endif