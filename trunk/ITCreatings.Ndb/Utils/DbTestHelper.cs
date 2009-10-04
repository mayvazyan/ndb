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

        /// <summary>
        /// Provides direct access to the underlayed DbGateway
        /// </summary>
        public DbGateway gateway { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTestHelper"/> class.
        /// </summary>
        /// <param name="Gateway">The gateway.</param>
        public DbTestHelper(DbGateway Gateway)
        {
            gateway = Gateway;
            toRemove = new List<object>();
        }

        /// <summary>
        /// Inserts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected void Insert(object data)
        {
            toRemove.Add(data);
            gateway.Insert(data);
        }

        /// <summary>
        /// Deletes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected void Delete(object data)
        {
            gateway.Delete(data);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    Delete(toRemove[i]);
                }
                toRemove.Clear();
            }
        }
    }
}
