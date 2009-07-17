using System.Collections.Generic;

namespace ITCreatings.Ndb.Query.Filters
{
    /// <summary>
    /// Filters Groups
    /// </summary>
    public abstract class DbFilterGroup : DbFilterNode
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public List<DbFilterNode> Nodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        protected DbFilterGroup(List<DbFilterNode> nodes)
        {
            Nodes = nodes;
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Add(DbFilterNode node)
        {
            Nodes.Add(node);
        }
    }
}