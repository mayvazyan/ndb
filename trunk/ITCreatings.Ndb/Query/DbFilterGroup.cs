using System.Collections.Generic;

namespace ITCreatings.Ndb.Query
{
    public class DbOrFilterGroup : DbFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbOrFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public DbOrFilterGroup(List<DbFilterNode> nodes) : base(nodes){}

        public DbOrFilterGroup(params DbFilterNode [] nodes) : base(new List<DbFilterNode>(nodes)){}
    }

    public class DbAndFilterGroup : DbFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbAndFilterGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public DbAndFilterGroup(List<DbFilterNode> nodes) : base(nodes) { }

        public DbAndFilterGroup(params DbFilterNode [] nodes) : base(new List<DbFilterNode>(nodes)){}
    }

    /// <summary>
    /// Filters Groups
    /// </summary>
    public abstract class DbFilterGroup : DbFilterNode
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public readonly List<DbFilterNode> Nodes;

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