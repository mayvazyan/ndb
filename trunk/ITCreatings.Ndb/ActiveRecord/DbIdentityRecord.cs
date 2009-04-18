using System;
using ITCreatings.Ndb.Attributes.Keys;

namespace ITCreatings.Ndb.ActiveRecord
{
    /// <summary>
    /// Base class for Identity objects
    /// </summary>
    [Serializable]
    public abstract class DbIdentityRecord : DbActiveRecord
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        [DbPrimaryKeyField]
        public ulong Id;

        /// <summary>
        /// Loads object from database by Primary Key
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            return DbGateway.Instance.Load(this, Id);
        }
    }
}