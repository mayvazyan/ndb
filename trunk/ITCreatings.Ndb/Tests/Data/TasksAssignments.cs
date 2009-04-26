#if DEBUG
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Indexes;
using ITCreatings.Ndb.Attributes.Keys;
using ITCreatings.Ndb.Tests.Data;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("TasksAssignments")]
    public class TasksAssignment : DbActiveRecord
    {
        private const string IndexName = "TaskUser";

        [DbForeignKeyField(typeof(Task))]
        [DbUniqueField(IndexName = IndexName)]
        public ulong TaskId;

        [DbUniqueField(IndexName = IndexName)]
        [DbForeignKeyField(typeof(User))]
        public ulong UserId;

        [DbParentRecord] public readonly Task Task;
        [DbParentRecord] public readonly User User;
    }
}
#endif