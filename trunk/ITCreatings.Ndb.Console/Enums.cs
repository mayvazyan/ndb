namespace ITCreatings.Ndb.NdbConsole
{
    /// <summary>
    /// Error codes returned by NdbConsole
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        /// 
        /// </summary>
        Success = 0,
        /// <summary>
        /// 
        /// </summary>
        Exception
    }

    public enum Action
    {
        /// <summary>
        /// Create database structure
        /// </summary>
        Create,


        /// <summary>
        /// Drop database objects
        /// </summary>
        Drop,


        /// <summary>
        /// Update database objects
        /// </summary>
        Alter,

        /// <summary>
        /// Generate Classes depending on the database structure
        /// </summary>
        Generate,

        /// <summary>
        /// Check is database structure and code are synchronized
        /// </summary>
        Check,
    }
}