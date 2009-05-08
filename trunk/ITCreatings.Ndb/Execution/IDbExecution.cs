namespace ITCreatings.Ndb.Execution
{
    /// <summary>
    /// Interface of the DbExecutor's worker
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TExecutionResultCode">The type of the error.</typeparam>
    public interface IDbExecution<TResult, TExecutionResultCode>
    {
        /// <summary>
        /// Gets or sets the result code.
        /// </summary>
        /// <value>The result code.</value>
        TExecutionResultCode PossibleResultCode { get; set; }

        /// <summary>
        /// Error container
        /// </summary>
        DbExecutionError<TExecutionResultCode> Error { get; set; }

        /// <summary>
        /// Result container
        /// </summary>
        TResult Result { get; set; }

        /// <summary>
        /// Accessor to underlayed logger
        /// </summary>
        log4net.ILog Logger { get; }
    }
}