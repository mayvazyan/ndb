namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Order info holder
    /// </summary>
    public class DbOrder
    {
        /// <summary>
        /// Order Direction
        /// </summary>
        public DbSortingDirection SortingDirection { get; private set; }

        /// <summary>
        /// Column Name to sort by
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbOrder()
        {}

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sortingDirection"></param>
        public DbOrder(string columnName, DbSortingDirection sortingDirection)
        {
            ColumnName = columnName;
            SortingDirection = sortingDirection;
        }
    }
}
