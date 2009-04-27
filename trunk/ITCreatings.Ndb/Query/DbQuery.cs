using System.Collections.Generic;
using System.Text;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// SQL Query builder
    /// </summary>
    /// <typeparam name="T">Type to Load</typeparam>
    public class DbQuery<T>
    {
        /// <summary>
        /// List of Filter Expressions
        /// </summary>
        public List<DbFilterExpression> FilterExpressions { get; private set; }

        private DbOrder Order;
        private int limit;
        private int offset;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbQuery() : this(new List<DbFilterExpression>())
        {
        }

        /// <summary>
        /// Allows to specify Filter Expression
        /// </summary>
        /// <param name="filterExpressions"></param>
        public DbQuery(List<DbFilterExpression> filterExpressions)
        {
            FilterExpressions = filterExpressions;
        }

        #region Paging

        /// <summary>
        /// Sets records limit
        /// </summary>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public DbQuery<T> Limit(int Limit)
        {
            limit = Limit;
            return this;
        }

        /// <summary>
        /// Sets records offset
        /// </summary>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public DbQuery<T> Offset(int Offset)
        {
            offset = Offset;
            return this;
        }

        #endregion

        #region OrderBy

        /// <summary>
        /// Sets column name to sort by
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public DbQuery<T> OrderBy(string columnName)
        {
            return OrderBy(columnName, DbSortingDirection.Asc);
        }
        
        /// <summary>
        /// Sets column name sort by and sorting direction
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sortingDirection"></param>
        /// <returns></returns>
        public DbQuery<T> OrderBy(string columnName, DbSortingDirection sortingDirection)
        {
            Order = new DbOrder(columnName, sortingDirection);
            return this;
        }

        #endregion

        #region DbFilterExpression helpers

        /// <summary>
        /// Adds Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DbQuery<T> Add(DbFilterExpression expression)
        {
            FilterExpressions.Add(expression);
            return this;
        }

        /// <summary>
        /// Adds Is Null Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DbQuery<T> IsNull(string Name)
        {
            return Add(new DbFilterExpression(DbExpressionType.IsNull, Name));
        }

        /// <summary>
        /// Adds Is Not Null Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DbQuery<T> IsNotNull(string Name)
        {
            return Add(new DbFilterExpression(DbExpressionType.IsNotNull, Name));
        }

        #endregion

        #region DbColumnFilterExpression helpers

        /// <summary>
        /// Adds Contains Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> Contains(string Name, string Value)
        {
            return Add(DbExpressionType.Contains, Name, Value);
            
        }

        /// <summary>
        /// Adds Starts With Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> StartsWith(string Name, string Value)
        {
            return Add(DbExpressionType.StartsWith, Name, Value);
        }

        /// <summary>
        /// Adds Ends With Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> EndsWith(string Name, string Value)
        {
            return Add(DbExpressionType.EndsWith, Name, Value);
        }

        /// <summary>
        /// Adds Equals Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> Equals(string Name, object Value)
        {
            return Add(DbExpressionType.Equal, Name, Value);
        }

        /// <summary>
        /// Adds Greater Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> Greater(string Name, object Value)
        {
            return Add(DbExpressionType.Greater, Name, Value);
        }

        /// <summary>
        /// Adds Greater Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> Less(string Name, object Value)
        {
            return Add(DbExpressionType.Less, Name, Value);
        }

        private DbQuery<T> Add(DbExpressionType expressionType, string name, object value)
        {
            return Add(new DbColumnFilterExpression(expressionType, name, value));
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns></returns>
        public static DbQuery<T> Create()
        {
            return new DbQuery<T>();
        }

        #endregion

        /// <summary>
        /// Loads Array of objects
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public T[] Load(DbGateway gateway)
        {
            string tableName = DbAttributesManager.GetTableName(typeof(T));
            
            StringBuilder sb = new StringBuilder("SELECT * FROM ");
            sb.Append(tableName);

            object [] args = buildWhere(sb, gateway);
            buildOrderBy(sb);

            return gateway.LoadRecords<T>(sb.ToString(), limit, offset, args);
        }

        private void buildOrderBy(StringBuilder sb)
        {
            if (Order != null && !string.IsNullOrEmpty(Order.ColumnName))
            {
                sb.Append(string.Concat(" ORDER BY ", Order.ColumnName, " ", Order.SortingDirection));
            }
        }

        private object[] buildWhere(StringBuilder sb, DbGateway gateway)
        {
            int count = FilterExpressions.Count;

            var args = new List<object>(count*2);
            if (count > 0)
            {
                sb.Append(" WHERE ");

                sb.Append(FilterExpressions[0].ToString(gateway.Accessor, 0));
                FilterExpressions[0].AddParameters(args);

                for (int i = 1; i < count; i++)
                {
                    sb.Append(" AND ");
                    sb.Append(FilterExpressions[i].ToString(gateway.Accessor, i));
                    FilterExpressions[i].AddParameters(args);
                }
            }

            return args.ToArray();
        }
    }
}