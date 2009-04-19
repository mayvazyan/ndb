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
        /// Underlaid Db Gateway
        /// </summary>
        public DbGateway Gateway { get; private set; }

        /// <summary>
        /// Accessor table name
        /// </summary>
        protected string TableName { get { return DbAttributesManager.GetTableName(GetType()); } }

        /// <summary>
        /// Loads Record Count from Database
        /// </summary>
        public ulong Count { get { return Gateway.LoadCount(GetType()); } }

        /// <summary>
        /// By default ActiveRecord uses DbGateway.Instance in database access purposes
        /// </summary>
        protected DbActiveRecord()
            : this(DbGateway.Instance)
        {
        }
        
        ///<summary>
        /// Main constructor for ActiveRecord
        ///</summary>
        ///<param name="gateway"></param>
        protected DbActiveRecord(DbGateway gateway)
        {
            Gateway = gateway;
        }

        /// <summary>
        /// Removes Object from Database
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            return Gateway.Delete(this);
        }
        
        /// <summary>
        /// Stores Object to Database
        /// </summary>
        public void Save()
        {
            Gateway.Save(this);
        }

        /// <summary>
        /// Uses exact matching
        /// Example: Load("Status", 2, "DateCreate", DateTime.Today)
        /// </summary>
        /// <param name="args"></param>
        public bool LoadByMatch(params object[] args)
        {
            return Gateway.Load(this, args);
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
            return Gateway.Delete(GetType(), args);
        }

        /// <summary>
        /// Removes records filtered by ALL DbFields (uses exact matching)
        /// </summary>
        /// <returns>Removed Records Count</returns>
        protected uint DeleteByAllFields()
        {
            return Gateway.DeleteByAllFields(this);
        }

    }
}