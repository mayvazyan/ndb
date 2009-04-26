using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Once column filter expression
    /// </summary>
    public class DbColumnFilterExpression : DbFilterExpression
    {
        /// <summary>
        /// Value to use in Filter
        /// </summary>
        public object Value;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbColumnFilterExpression()
        {
        }

        /// <summary>
        /// Extended constructor
        /// </summary>
        /// <param name="expressionType"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public DbColumnFilterExpression(DbExpressionType expressionType, string columnName, object value)
            : base(expressionType, columnName)
        {
            Value = value;
        }

        internal override string ToString(DbAccessor accessor)
        {
            switch (ExpressionType)
            {
                case DbExpressionType.Equal:
                    return string.Concat(ColumnName, "='", Value, '\'');

                case DbExpressionType.Contains:
                    return string.Concat(ColumnName, " LIKE '%", Value, "%'");

                case DbExpressionType.EndsWith:
                    return string.Concat(ColumnName, " LIKE '%", Value, "'");

                case DbExpressionType.StartsWith:
                    return string.Concat(ColumnName, " LIKE '", Value, "%'");

                case DbExpressionType.Greater:
                    return string.Concat(ColumnName, ">", Value);

                case DbExpressionType.Less:
                    return string.Concat(ColumnName, "<", Value);

                default:
                    throw new DbInvalidFilterException(ExpressionType, GetType());
            }
        }
    }
}