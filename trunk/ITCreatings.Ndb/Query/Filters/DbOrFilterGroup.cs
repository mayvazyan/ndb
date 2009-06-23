using System.Collections.Generic;

namespace ITCreatings.Ndb.Query.Filters
{
    /// <summary>
    /// Represents OR filters group
    /// </summary>
    public class DbOrFilterGroup : DbFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbOrFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public DbOrFilterGroup(List<DbFilterNode> nodes) : base(nodes){}

        /// <summary>
        /// Initializes a new instance of the <see cref="DbOrFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public DbOrFilterGroup(params DbFilterNode [] nodes) : this(new List<DbFilterNode>(nodes)){}
    }
}