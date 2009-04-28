using System.Collections.Generic;
using System.Text;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// SQL Query builder
    /// </summary>
    /// <typeparam name="T">Type to Load</typeparam>
    public class DbQuery<T>
    {
        #region Data

        /// <summary>
        /// List of Filter Expressions
        /// </summary>
        public List<DbFilterExpression> FilterExpressions { get; private set; }

        /// <summary>
        /// Underlayed gateway.
        /// </summary>
        /// <value>The gateway.</value>
        public DbGateway Gateway { get; private set; }
        private DbOrder Order;
        private int limit;
        private int offset;

        #endregion

        #region Constructors & Factory methods

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        private DbQuery(DbGateway gateway)
            : this(gateway, new List<DbFilterExpression>())
        {
        }

        /// <summary>
        /// Allows to specify Filter Expression
        /// </summary>
        /// <param name="filterExpressions"></param>
        private DbQuery(DbGateway gateway, List<DbFilterExpression> filterExpressions)
        {
            foreach (DbFilterExpression expression in filterExpressions)
            {
                if (!Utils.DbValidator.IsValidColumnName(expression.ColumnName))
                    throw new NdbInvalidColumnNameException(expression.ColumnName);
            }

            Gateway = gateway;
            FilterExpressions = filterExpressions;
        }

        #endregion 

        #region Factory methods

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static DbQuery<T> Create(DbGateway gateway)
        {
            return new DbQuery<T>(gateway);
        }

        /// <summary>
        /// Creates this instance using specified filter expressions.
        /// </summary>
        /// <param name="gateway">The gateway</param>
        /// <param name="filterExpressions">The filter expressions.</param>
        /// <returns></returns>
        public static DbQuery<T> Create(DbGateway gateway, List<DbFilterExpression> filterExpressions)
        {
            return new DbQuery<T>(gateway, filterExpressions);
        }

        #endregion

        #endregion

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

        /// <summary>
        /// Sets records limit and offset
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public DbQuery<T> Limit(int limit, int offset)
        {
            return Limit(limit)
                .Offset(offset);
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

        #region Load & query build

        /// <summary>
        /// Loads Array of objects
        /// </summary>
        /// <returns></returns>
        public T[] Load()
        {
            string tableName = DbAttributesManager.GetTableName(typeof(T));
            
            StringBuilder sb = new StringBuilder("SELECT * FROM ");
            sb.Append(tableName);

            object [] args = buildWhere(sb, Gateway);
            buildOrderBy(sb);

            return Gateway.LoadRecords<T>(sb.ToString(), limit, offset, args);
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

        #endregion
    }
}