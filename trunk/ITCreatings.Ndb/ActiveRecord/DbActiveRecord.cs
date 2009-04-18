using System;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.ActiveRecord
{
    /// <summary>
    /// Implements Active Record Pattern.
    /// Contains Save (Insert, Update), Delete and Load functionality
    /// </summary>
    [Serializable]
    public abstract class DbActiveRecord
    {
        /// <summary>
        /// Accessor table name
        /// </summary>
        protected string TableName { get { return DbAttributesManager.GetTableName(GetType()); } }

        /// <summary>
        /// Helper accessor
        /// </summary>
        protected static DbAccessor Db { get { return DbAccessor.Instance; } }

        /// <summary>
        /// Loads Record Count from Database
        /// </summary>
        public ulong Count { get { return DbGateway.Instance.LoadCount(GetType()); } }

        /// <summary>
        /// Removes Object from Database
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            return DbGateway.Instance.Delete(this);
        }
        
        /// <summary>
        /// Stores Object to Database
        /// </summary>
        public void Save()
        {
            DbGateway.Instance.Save(this);
        }

        /// <summary>
        /// Uses exact matching
        /// Example: Load("Status", 2, "DateCreate", DateTime.Today)
        /// </summary>
        /// <param name="args"></param>
        public bool LoadByMatch(params object[] args)
        {
            return DbGateway.Instance.Load(this, args);
        }

        /// <summary>
        /// Uses exact matching
        /// Example: 
        /// </summary>
        /// <param name="args"></param>
        /// <example>
        /// <code>
        /// myActiveRecord.Delete("Status", 2, "DateCreated", DateTime.Today)
        /// </code>
        /// </example>
        public uint Delete(params object[] args)
        {
            return DbGateway.Instance.Delete(GetType(), args);
        }

        /// <summary>
        /// Removes records filtered by ALL DbFields (uses exact matching)
        /// </summary>
        /// <returns>Removed Records Count</returns>
        protected uint DeleteByAllFields()
        {
            return DbGateway.Instance.DeleteByAllFields(this);
        }

    }
}