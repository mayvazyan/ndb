#if DEBUG
using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Indexes;
using ITCreatings.Ndb.Attributes.Keys;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord]
    public class User : DbActiveRecord
    {
        [DbPrimaryKeyField] public UInt64 Id;

        [DbField]
        public RolesManager.Ids RoleId;

        [DbUniqueField]
        public string Email;

        [DbField]
        public string Password;

        [DbIndexedField]
        public string FirstName;

        [DbIndexedField]
        public string LastName;

        /// <summary>
        /// Date of Birthday
        /// </summary>
        [DbField]
        public DateTime Dob;

        public string FullName { get { return FirstName + " " + LastName; } }

        [DbChildRecords] public Event[] Events;
        [DbChildRecords] public TasksAssignment[] TasksAssignments;
    }
}
#endif