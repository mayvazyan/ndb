using System.Collections.Generic;
using System.Text;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// SQL Query builder
    /// </summary>
    public class DbQuery
    {
        #region Data

        /// <summary>
        /// List of Filter Expressions
        /// </summary>
        public DbFilterGroup FilterGroup { get; private set; }

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
        /// Initializes a new instance of the <see cref="DbQuery"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="filterExpressions">The filter expressions.</param>
        private DbQuery(DbGateway gateway, List<DbFilterNode> filterExpressions)
        {
            foreach (DbFilterExpression expression in filterExpressions)
            {
                if (!Utils.DbValidator.IsValidColumnName(expression.ColumnName))
                    throw new NdbInvalidColumnNameException(expression.ColumnName);
            }

            Gateway = gateway;
            FilterGroup = new DbAndFilterGroup(filterExpressions);
        }

        private DbQuery(DbGateway gateway) :
            this(gateway, new DbAndFilterGroup(new List<DbFilterNode>()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbQuery"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="filterGroup">The filter group.</param>
        private DbQuery(DbGateway gateway, DbFilterGroup filterGroup)
        {
            //TODO: add filters validation?

            Gateway = gateway;
            FilterGroup = filterGroup;
        }


        #endregion 

        #region Factory methods

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static DbQuery Create(DbGateway gateway)
        {
            return new DbQuery(gateway);
        }

        /// <summary>
        /// Creates this instance using specified filter expressions.
        /// </summary>
        /// <param name="gateway">The gateway</param>
        /// <param name="filterExpressions">The filter expressions.</param>
        /// <returns></returns>
        public static DbQuery Create(DbGateway gateway, List<DbFilterNode> filterExpressions)
        {
            return new DbQuery(gateway, filterExpressions);
        }

        public static DbQuery Create(DbGateway gateway, DbFilterGroup filterGroup)
        {
            return new DbQuery(gateway, filterGroup);
        }

        #endregion

        #endregion

        #region Paging

        /// <summary>
        /// Sets records limit
        /// </summary>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public DbQuery Limit(int Limit)
        {
            limit = Limit;
            return this;
        }

        /// <summary>
        /// Sets records offset
        /// </summary>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public DbQuery Offset(int Offset)
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
        public DbQuery Limit(int limit, int offset)
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
        public DbQuery OrderBy(string columnName)
        {
            return OrderBy(columnName, DbSortingDirection.Asc);
        }
        
        /// <summary>
        /// Sets column name sort by and sorting direction
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sortingDirection"></param>
        /// <returns></returns>
        public DbQuery OrderBy(string columnName, DbSortingDirection sortingDirection)
        {
            Order = new DbOrder(columnName, sortingDirection);
            return this;
        }

        /// <summary>
        /// Sets column name sort by and sorting direction
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public DbQuery OrderBy(DbOrder order)
        {
            Order = order;
            return this;
        }

        #endregion

        #region DbFilterExpression helpers

        /// <summary>
        /// Adds Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DbQuery Add(DbFilterNode expression)
        {
            FilterGroup.Add(expression);
            return this;
        }

        /// <summary>
        /// Adds Is Null Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DbQuery IsNull(string Name)
        {
            return Add(new DbFilterExpression(DbExpressionType.IsNull, Name));
        }

        /// <summary>
        /// Adds Is Not Null Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DbQuery IsNotNull(string Name)
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
        public DbQuery Contains(string Name, string Value)
        {
            return Add(DbExpressionType.Contains, Name, Value);
            
        }

        /// <summary>
        /// Adds Starts With Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery StartsWith(string Name, string Value)
        {
            return Add(DbExpressionType.StartsWith, Name, Value);
        }

        /// <summary>
        /// Adds Ends With Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery EndsWith(string Name, string Value)
        {
            return Add(DbExpressionType.EndsWith, Name, Value);
        }

        /// <summary>
        /// Adds Equals Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery Equals(string Name, object Value)
        {
            return Add(DbExpressionType.Equal, Name, Value);
        }

        /// <summary>
        /// Adds Not Equal Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery NotEqual(string Name, object Value)
        {
            return Add(DbExpressionType.NotEqual, Name, Value);
        }

        /// <summary>
        /// Adds Greater Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery Greater(string Name, object Value)
        {
            return Add(DbExpressionType.Greater, Name, Value);
        }

        /// <summary>
        /// Adds Greater Expression
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public DbQuery Less(string Name, object Value)
        {
            return Add(DbExpressionType.Less, Name, Value);
        }

        private DbQuery Add(DbExpressionType expressionType, string name, object value)
        {
            return Add(new DbColumnFilterExpression(expressionType, name, value));
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads Array of objects
        /// </summary>
        /// <returns></returns>
        public T[] Load<T>()
        {
            DbRecordInfo recordInfo = DbAttributesManager.GetRecordInfo(typeof(T));
            
            var sb = new StringBuilder();
            DbQueryBuilder.BuildSelect(sb, recordInfo);

            object [] args = buildWhere(sb, Gateway);
            buildOrderBy(sb);

            return Gateway.LoadRecords<T>(sb.ToString(), limit, offset, args);
        }

        /// <summary>
        /// Records count.
        /// </summary>
        /// <returns></returns>
        public ulong LoadCount<T>()
        {
            DbRecordInfo recordInfo = DbAttributesManager.GetRecordInfo(typeof(T));
            
            var sb = new StringBuilder();
            DbQueryBuilder.BuildSelectCount(sb, recordInfo);

            object [] args = buildWhere(sb, Gateway);
            buildOrderBy(sb);

            return Gateway.LoadResult<ulong>(sb.ToString(), args);
        }

        /// <summary>
        /// Loads the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="LoadTotalCount">if set to <c>true</c> [load total count].</param>
        /// <returns></returns>
        public DbQueryResult<T> LoadResult<T>(bool LoadTotalCount)
        {
            var result = new DbQueryResult<T>();
            if (!LoadTotalCount)
            {
                result.Records = Load<T>();
            }
            else
            {
                //TODO: (high priority) refactor
                DbRecordInfo recordInfo = DbAttributesManager.GetRecordInfo(typeof (T));

                var sb = new StringBuilder();
                DbQueryBuilder.BuildSelect(sb, recordInfo);

                object[] args = buildWhere(sb, Gateway);
                buildOrderBy(sb);

                sb.Insert(7, "SQL_CALC_FOUND_ROWS ");

                result.Records = Gateway.LoadRecords<T>(sb.ToString(), limit, offset, args);
                result.TotalRecordsCount = Gateway.LoadResult<long>("SELECT FOUND_ROWS()");
            }
            return result;
        }

        #endregion

        #region Query Building

        private void buildOrderBy(StringBuilder sb)
        {
            if (Order != null && !string.IsNullOrEmpty(Order.ColumnName))
            {
                sb.Append(string.Concat(" ORDER BY ", Order.ColumnName, " ", Order.SortingDirection));
            }
        }

        private object[] buildWhere(StringBuilder sb, DbGateway gateway)
        {
            var args = new List<object>();
            var builder = new DbFilterBuilder(sb, gateway, args);
            builder.Build(FilterGroup);
            return args.ToArray();
            
        }

        #endregion
    }
}