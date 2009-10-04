using System.Collections.Generic;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query.Filters
{
    /// <summary>
    /// Once column expressions holder (for example IsNull, NotNull)
    /// </summary>
    public class DbFilterExpression : DbFilterNode
    {
        /// <summary>
        /// Expression Type
        /// </summary>
        public DbExpressionType ExpressionType { get; private set; }

        /// <summary>
        /// Column Name
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbFilterExpression()
        {
        }

        /// <summary>
        /// Extended constructor
        /// </summary>
        /// <param name="expressionType"></param>
        /// <param name="columnName"></param>
        public DbFilterExpression(DbExpressionType expressionType, string columnName)
        {
            ExpressionType = expressionType;
            ColumnName = columnName;
        }

        internal virtual string ToString(DbAccessor accessor, int index)
        {
            switch(ExpressionType)
            {
                case DbExpressionType.IsNull:
                    return ColumnName + " IS NULL";

                case DbExpressionType.IsNotNull:
                    return ColumnName + " IS NOT NULL";

                default:
                    throw new NdbInvalidFilterException(ExpressionType, GetType());
            }
        }

        internal virtual void AddParameters(List<object> args)
        {
        }
    }
}