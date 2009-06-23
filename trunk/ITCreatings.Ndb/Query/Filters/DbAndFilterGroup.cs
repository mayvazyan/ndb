using System.Collections.Generic;

namespace ITCreatings.Ndb.Query.Filters
{
    /// <summary>
    /// Represents AND filters group
    /// </summary>
    public class DbAndFilterGroup : DbFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbAndFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public DbAndFilterGroup(List<DbFilterNode> nodes) : base(nodes) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAndFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public DbAndFilterGroup(params DbFilterNode [] nodes) : this(new List<DbFilterNode>(nodes)){}
    }
}