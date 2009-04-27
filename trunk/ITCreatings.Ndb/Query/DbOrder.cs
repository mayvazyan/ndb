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
        public DbSortingDirection SortingDirection;

        /// <summary>
        /// Column Name to sort by
        /// </summary>
        public string ColumnName;

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
