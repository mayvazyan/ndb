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

        /// <summary>
        /// Sorting info
        /// </summary>
        public DbSorting Sorting { get; private set; }

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

        /// <summary>
        /// Sets column name to sort by
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public DbQuery<T> Sort(string columnName)
        {
            return Sort(columnName, DbSortingDirection.Asc);
        }
        
        /// <summary>
        /// Sets column name sort by and sorting direction
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sortingDirection"></param>
        /// <returns></returns>
        public DbQuery<T> Sort(string columnName, DbSortingDirection sortingDirection)
        {
            Sorting = new DbSorting(columnName, sortingDirection);
            return this;
        }

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
        
        /// <summary>
        /// Adds Contains Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery<T> Contains(string Name, string Value)
        {
            return Add(new DbColumnFilterExpression(DbExpressionType.Contains, Name, Value));
        }

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
            int Limit = 0;
            int Offset = 0;

            string tableName = DbAttributesManager.GetTableName(typeof(T));
            
            StringBuilder sb = new StringBuilder("SELECT * FROM ");
            sb.Append(tableName);

            buildWhere(sb, gateway);

            return gateway.LoadRecords<T>(sb.ToString(), Limit, Offset);
        }

        private void buildWhere(StringBuilder sb, DbGateway gateway)
        {
            int count = FilterExpressions.Count;

            if (count > 0)
            {
                sb.Append(" WHERE ");
                
                sb.Append(FilterExpressions[0].ToString(gateway.Accessor));
                for (int i = 1; i < count; i++)
                {
                    sb.Append(" AND ");
                    sb.Append(FilterExpressions[0].ToString(gateway.Accessor));
                }
            }
        }
    }
}