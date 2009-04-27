namespace ITCreatings.Ndb.Execution
{
    /// <summary>
    /// Interface of the worker
    /// </summary>
    public interface IDbExecution<T>
    {
        /// <summary>
        /// Error container
        /// </summary>
        DbExecutionError Error { get; set; }

        /// <summary>
        /// Result container
        /// </summary>
        T Result { get; set; }

        /// <summary>
        /// Accessor to underlayed logger
        /// </summary>
        log4net.ILog Logger { get; }
    }
}