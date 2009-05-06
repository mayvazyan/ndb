using System;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Represents the query result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbQueryResult<T>
    {
        /// <summary>
        /// Gets or sets the total records count.
        /// </summary>
        /// <value>The total records count.</value>
        public long TotalRecordsCount { get; set; }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        public T[] Records { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbQueryResult&lt;T&gt;"/> class.
        /// </summary>
        public DbQueryResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbQueryResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="totalRecordsCount">The total records count.</param>
        public DbQueryResult(T[] records, long totalRecordsCount)
        {
            Records = records;
            TotalRecordsCount = totalRecordsCount;
        }

        /// <summary>
        /// Gets the pages count.
        /// </summary>
        /// <param name="perPage">The per page.</param>
        /// <returns></returns>
        public double GetPagesCount(int perPage)
        {
            return Math.Ceiling(TotalRecordsCount/(double) perPage);
        }
    }
}
