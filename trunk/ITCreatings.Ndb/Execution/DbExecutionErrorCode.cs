namespace ITCreatings.Ndb.Execution
{
    /// <summary>
    /// Execution Error Code
    /// </summary>
    public enum DbExecutionErrorCode
    {
        /// <summary>
        /// Means custom error
        /// </summary>
        Custom,

        /// <summary>
        /// Represents NdbConnectionFailedException
        /// </summary>
        ConnectionFailed
    }
}