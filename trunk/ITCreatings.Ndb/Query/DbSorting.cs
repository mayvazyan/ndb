namespace ITCreatings.Ndb.Query
{
    /// <summary>
    /// Sorting info holder
    /// </summary>
    public class DbSorting
    {
        /// <summary>
        /// Sorting Direction
        /// </summary>
        public DbSortingDirection SortingDirection;

        /// <summary>
        /// Column Name to sort by
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbSorting()
        {}

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sortingDirection"></param>
        public DbSorting(string columnName, DbSortingDirection sortingDirection)
        {
            ColumnName = columnName;
            SortingDirection = sortingDirection;
        }
    }
}
