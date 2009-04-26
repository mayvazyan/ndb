namespace ITCreatings.Ndb.Execution
{
    /// <summary>
    /// Interface of the worker
    /// </summary>
    public interface IExecution<T>
    {
        /// <summary>
        /// Error container
        /// </summary>
        ExecutionError Error { get; set; }

        /// <summary>
        /// Result container
        /// </summary>
        T Result { get; set; }
    }
}