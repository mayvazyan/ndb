using System;
using ITCreatings.Ndb.Attributes;

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
        public long Id;

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