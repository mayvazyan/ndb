using System;
using System.Reflection;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Tests
{
    /// <summary>
    /// Contains a set of helpers methods for your Unit Tests
    /// </summary>
    public class DbTestUtils
    {
        private readonly DbGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTestUtils"/> class.
        /// </summary>
        /// <param name="accessor">The accessor.</param>
        public DbTestUtils(DbAccessor accessor)
        {
            gateway = new DbGateway(accessor);
        }
        /// <summary>
        /// Checks all objects with DbFieldAttribute to match db
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public bool IsDbRecordsValid(Assembly assembly)
        {
            Type[] types = DbAttributesManager.LoadDbRecordTypes(assembly);

            foreach (Type type in types)
                if (!DbStructureGateway.Instance.IsValid(type))
                    return false;

            return true;
        }

        /// <summary>
        /// Checks all classes with DbRecordAttribute to match to associated database tables
        /// </summary>
        /// <param name="assembly"></param>
        public bool CheckDbRecordTypes(Assembly assembly)
        {
            Type[] types = DbAttributesManager.LoadDbRecordTypes(assembly);
            foreach (var type in types)
            {
                if (!DbStructureGateway.Instance.IsValid(type))
                    throw new NdbException(
                    string.Format("Not all fields of Type {0} match associated database table:\r\n{1}"
                    , type
                    , DbStructureGateway.Instance.LastError));
            }

            return true;
        }

        /// <summary>
        /// Returns whole seconds difference between passed Dates
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public UInt32 DiffInSeconds(DateTime dateTime1, DateTime dateTime2)
        {
            TimeSpan diff = dateTime1.Date.Subtract(dateTime2.Date);
            return Convert.ToUInt32(diff.TotalSeconds);
        }

        /// <summary>
        /// Returns whole seconds difference between passed Dates
        /// </summary>
        /// <param name="dateTime1">The date time1.</param>
        /// <param name="dateTime2">The date time2.</param>
        /// <returns></returns>
        public UInt32 DiffInSeconds(DateTime? dateTime1, DateTime? dateTime2)
        {
            return DiffInSeconds(dateTime1.Value, dateTime2.Value);
        }

        #region Helpers

        /// <summary>
        /// Updates object in database and check what records counts wasn't changed
        /// </summary>
        /// <param name="data"></param>
        public bool UpdateTest(object data)
        {
            var count = gateway.LoadCount(data.GetType());
            gateway.Save(data);
            return count == gateway.LoadCount(data.GetType());
        }

        /// <summary>
        /// adds new record to database and checks what records count was incremented
        /// </summary>
        /// <param name="data"></param>
        public bool SaveTest(object data)
        {
            var count = gateway.LoadCount(data.GetType());
            gateway.Save(data);
            return ++count == gateway.LoadCount(data.GetType());
        }

        /// <summary>
        /// Removes object from Database and checks count (should be "-1")
        /// </summary>
        /// <param name="data"></param>
        public bool DeleteTest(object data)
        {
            var count = gateway.LoadCount(data.GetType());
            gateway.Delete(data);
            return --count == gateway.LoadCount(data.GetType());
        }

        #endregion
    }
}