namespace ITCreatings.Ndb.NdbConsole
{
    /// <summary>
    /// Error codes returned by NdbConsole
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 0,

        /// <summary>
        /// Exception
        /// </summary>
        Exception,

        /// <summary>
        /// Failure
        /// </summary>
        Failure
    }

    public enum Action
    {
        /// <summary>
        /// Creates database structure
        /// </summary>
        Create,


        /// <summary>
        /// Drops database objects
        /// </summary>
        Drop,


        /// <summary>
        /// Updates database objects
        /// </summary>
        Alter,

        /// <summary>
        /// Generates Classes depending on the database structure
        /// </summary>
        Generate,

        /// <summary>
        /// Checks is database structure and code are synchronized
        /// </summary>
        Check,

        /// <summary>
        /// Recreates database structure
        /// </summary>
        Recreate,

        /// <summary>
        /// Imports data from Excel file (removes presend data)
        /// </summary>
        ImportExcel
    }
}