using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Two columns expressions holder.
    /// </summary>
    public class DbColumnToColumnFilterExpression : DbFilterExpression
    {
        /// <summary>
        /// Second Column Name
        /// </summary>
        public string ColumnName2;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbColumnToColumnFilterExpression()
        {
        }

        /// <summary>
        /// Extended constructor
        /// </summary>
        /// <param name="expressionType"></param>
        /// <param name="columnName"></param>
        /// <param name="columnName2"></param>
        public DbColumnToColumnFilterExpression(DbExpressionType expressionType, string columnName, string columnName2)
            : base(expressionType, columnName)
        {
            ColumnName2 = columnName2;
        }

        internal override string ToString(DbAccessor accessor, int paramIndex)
        {
            switch (ExpressionType)
            {
                case DbExpressionType.Equal:
                    return string.Concat(ColumnName, "=", ColumnName2);

                case DbExpressionType.Greater:
                    return string.Concat(ColumnName, ">", ColumnName2);

                case DbExpressionType.Less:
                    return string.Concat(ColumnName, "<", ColumnName2);

                default:
                    throw new NdbInvalidFilterException(ExpressionType, GetType());
            }
        }
    }
}