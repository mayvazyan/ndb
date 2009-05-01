#if DEBUG
using System;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("Events")]
    public class Event
    {
        [DbPrimaryKeyField]
        public ulong Id;

        [DbForeignKeyField(typeof(User))]
        public ulong UserId;

        [DbField]
        public EventType EventTypeId;

        [DbField]
        public DateTime Timestamp;

        [DbParentRecord] public User User;
    }
}
#endif