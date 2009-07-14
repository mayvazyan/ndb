using System;
using System.Collections.Generic;

namespace ITCreatings.Ndb.Utils
{
    /// <summary>
    /// Utils class for unit tests
    /// <example>
    /// <code>
    /// using (DbTestUtils testUtils = new DbTestUtils(gateway)
    /// {
    ///     testUtils.Insert(TestData.GetUser());
    ///     testUtils.Insert(TestData.GetWorkLog());
    /// 
    ///     ... //Some asserts here
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class DbTestHelper : IDisposable
    {
        private readonly List<object> toRemove;
        public readonly DbGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTestHelper"/> class.
        /// </summary>
        /// <param name="Gateway">The gateway.</param>
        public DbTestHelper(DbGateway Gateway)
        {
            gateway = Gateway;
            toRemove = new List<object>();
        }

        protected void Insert(object data)
        {
            toRemove.Add(data);
            gateway.Insert(data);
        }

        protected void Delete(object data)
        {
            gateway.Delete(data);
        }

        public void Dispose()
        {
            for (int i = toRemove.Count - 1; i >=0; i--)
            {
                Delete(toRemove[i]);
            }
        }
    }
}
