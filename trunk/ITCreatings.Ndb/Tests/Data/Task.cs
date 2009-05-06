#if DEBUG
using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord]
    public class Task : DbIdentityRecord
    {
        [DbPrimaryKeyField]
        public int ProjectId;

        [DbField]
        public short TypeId;

        [DbField]
        public string Title;

        [DbField]
        public int EstimatedMinutes;

        [DbField]
        public string Description;

        [DbField]
        public short PriorityId;

        [DbField]
        public DateTime? DueDate;

        [DbField]
        public DateTime? Timestamp;

        [DbField] public bool IsDone;
        [DbField] public double SpentMinutes;
        [DbField] public decimal Price;

        [DbChildRecords] public TasksAssignment[] TasksAssignment;
    }
}
#endif