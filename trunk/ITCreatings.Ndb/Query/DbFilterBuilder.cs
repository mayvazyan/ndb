using System.Collections.Generic;
using System.Text;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Filters Builder
    /// </summary>
    public class DbFilterBuilder
    {
        private List<object> args;
        private StringBuilder sb;
        private DbGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFilterBuilder"/> class.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="gateway">The gateway.</param>
        /// <param name="args">The args.</param>
        public DbFilterBuilder(StringBuilder sb, DbGateway gateway, List<object> args)
        {
            this.args = args;
            this.sb = sb;
            this.gateway = gateway;
        }

        /// <summary>
        /// Builds the specified filter node.
        /// </summary>
        /// <param name="filterNode">The filter node.</param>
        public void Build(DbFilterGroup filterNode)
        {
            if (filterNode.Nodes.Count > 0)
            {
                sb.Append(" WHERE ");
                build(filterNode);
            }
        }

        private void build(DbFilterNode filterNode)
        {
            
            if (filterNode is DbFilterGroup)
            {
                string expression = (filterNode is DbAndFilterGroup) ? " AND " : " OR ";

                DbFilterGroup filterGroup = (DbFilterGroup) filterNode;

                sb.Append('(');
                int count = filterGroup.Nodes.Count;
                for (int i = 0; i < count; i++)
                {
                    var node = filterGroup.Nodes[i];
                    build(node);

                    if (i < count - 1)
                        sb.Append(expression);
                }
                sb.Append(')');
            }
            else 
            {
                DbFilterExpression exp = (DbFilterExpression) filterNode;
                sb.Append(exp.ToString(gateway.Accessor, args.Count));
                exp.AddParameters(args);
            }

                /*sb.Append(filterNode[0].ToString(gateway.Accessor, 0));
                filterNode[0].AddParameters(args);

                for (int i = 1; i < count; i++)
                {
                    sb.Append(" AND ");
                    sb.Append(filterNode[i].ToString(gateway.Accessor, i));
                    filterNode[i].AddParameters(args);
                }*/
        }
    }
}
