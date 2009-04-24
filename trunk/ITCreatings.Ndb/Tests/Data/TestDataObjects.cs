#if DEBUG
using System;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Keys;

namespace ITCreatings.Ndb.Tests.Data
{
    public enum EventType : ushort
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
        public UInt32 Id;

        [DbField]
        public UInt32 TaskId;

        [DbForeignKeyField(typeof(TestUser))]
        public UInt64 UserId;

        [DbField]
        public uint SpentMinutes;
        [DbField(Name = "CreationDate")]
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
        public UInt32 Id;

        [DbField]
        public string Description2;
    }

    [DbRecord("WorkLogs")]
    public class TestWorkLogItem2
    {
        [DbPrimaryKeyField]
        public UInt32 Id;

        [DbField]
        public UInt32 TaskId;

        [DbForeignKeyField(typeof(TestUser))]
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