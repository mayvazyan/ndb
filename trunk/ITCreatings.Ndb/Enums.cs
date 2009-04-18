namespace ITCreatings.Ndb
{
    /// <summary>
    /// Represents an database engine
    /// </summary>
    public enum DbProvider
    {
        /// <summary>
        /// An analog of the Null value
        /// </summary>
        Unknown,

        /// <summary>
        /// MySQL database
        /// </summary>
        MySql,

        /// <summary>
        /// SQLite database
        /// </summary>
        SqLite,

        /// <summary>
        /// Postgre SQL database
        /// </summary>
        Postgre,

        /// <summary>
        /// MS SQL Server database
        /// </summary>
        MsSql,

        /// <summary>
        /// MS SQL Compact Edition database
        /// </summary>
        MsSqlCe
    }
}
