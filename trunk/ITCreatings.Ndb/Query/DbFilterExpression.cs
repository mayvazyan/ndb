using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Once column expressions holder (for example IsNull, NotNull)
    /// </summary>
    public class DbFilterExpression
    {
        /// <summary>
        /// Expression Type
        /// </summary>
        public DbExpressionType ExpressionType;

        /// <summary>
        /// Column Name
        /// </summary>
        public string ColumnName;

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

        internal virtual string ToString(DbAccessor accessor)
        {
            switch(ExpressionType)
            {
                case DbExpressionType.IsNull:
                    return ColumnName + " IS NULL";

                case DbExpressionType.IsNotNull:
                    return ColumnName + " IS NOT NULL";

                default:
                    throw new DbInvalidFilterException(ExpressionType, GetType());
            }
        }
    }
}