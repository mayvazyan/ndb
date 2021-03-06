﻿#if TESTS
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Tests.Data;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("TasksAssignments")]
    public class TasksAssignment : DbActiveRecord
    {
        private const string IndexName = "TaskUser";

        [DbForeignKeyField(typeof(Task))]
        [DbUniqueField(IndexName)]
        public long TaskId;

        [DbUniqueField(IndexName)]
        [DbForeignKeyField(typeof(User))]
        public long UserId;

        [DbParentRecord] public readonly Task Task;
        [DbParentRecord] public readonly User User;
    }
}
#endif