using System.Collections.Generic;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query.Filters
{
    /// <summary>
    /// Once column filter expression
    /// </summary>
    public class DbColumnFilterExpression : DbFilterExpression
    {
        /// <summary>
        /// Value to use in Filter
        /// </summary>
        public object Value { get; set; }

        private string ParamName;

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

        internal override string ToString(DbAccessor accessor, int paramIndex)
        {
            ParamName = string.Concat(ColumnName, "_uid", paramIndex);

            switch (ExpressionType)
            {
                case DbExpressionType.Equal:
                    return string.Concat(ColumnName, "=@", ParamName);

                case DbExpressionType.NotEqual:
                    return string.Concat(ColumnName, "<>@", ParamName);

                case DbExpressionType.StartsWith:
                    return string.Concat(ColumnName, " LIKE @", ParamName);

                case DbExpressionType.Contains:
                    return string.Concat(ColumnName, " LIKE @", ParamName);

                case DbExpressionType.EndsWith:
                    return string.Concat(ColumnName, " LIKE @", ParamName);


                case DbExpressionType.Greater:
                    return string.Concat(ColumnName, ">@", ParamName);

                case DbExpressionType.Less:
                    return string.Concat(ColumnName, "<@", ParamName);

                case DbExpressionType.GreaterOrEqual:
                    return string.Concat(ColumnName, ">=@", ParamName);

                case DbExpressionType.LessOrEqual:
                    return string.Concat(ColumnName, "<=@", ParamName);

                default:
                    throw new NdbInvalidFilterException(ExpressionType, GetType());
            }
        }

        internal override void AddParameters(List<object> args)
        {
            args.Add(ParamName);

            switch (ExpressionType)
            {
                case DbExpressionType.StartsWith:
                    args.Add(string.Concat(Value, '%'));
                    break;

                case DbExpressionType.Contains:
                    args.Add(string.Concat('%', Value, '%'));
                    break;

                case DbExpressionType.EndsWith:
                    args.Add(string.Concat('%', Value));
                    break;

                default:
                    args.Add(Value);
                    break;
            }
        }

    }
}