#if DEBUG
using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Keys;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord]
    public class Task : DbIdentityRecord
    {
        [DbPrimaryKeyField]
        public uint ProjectId;

        [DbField]
        public ushort TypeId;

        [DbField]
        public string Title;

        [DbField]
        public uint EstimatedMinutes;

        [DbField]
        public string Description;

        [DbField]
        public ushort PriorityId;

        [DbField]
        public DateTime DueDate;

        [DbField]
        public DateTime Timestamp;

        [DbField] public bool IsDone;
        [DbField] public double SpentMinutes;
        [DbField] public decimal Price;

        [DbChildRecords] public TasksAssignment[] TasksAssignment;
    }
}
#endif