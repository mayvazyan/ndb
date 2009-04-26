using System;

namespace ITCreatings.Ndb.Execution
{
    /// <summary>
    /// Execution flow helper
    /// <example>
    /// <code>
    /// var executor = DbExecution{User}.Create()
    ///         .Validate(user.Id == 0, message1)
    ///         .Validate(string.IsNullOrEmpty(user.Password), message2)
    ///         .Execute(delegate (IExecution{User} execution) { execution.Error = "Test"; });
    /// 
    /// bool isError = executor.IsError;
    /// User user = executor.Result;
    /// ExecutionError executionError = executor.ExecutionError;
    /// </code>
    /// </example>
    /// </summary>
    internal class DbExecution<T> : IExecution<T>
    {
        public T Result { get; set; }
        public ExecutionError Error { get; set;}
        
        public bool IsError { get { return Error != null; } }
        
        public delegate T ExecuteDelegate(object data, IExecution<T> execution);

        #region Factory

        public static DbExecution<T> Create()
        {
            return new DbExecution<T>();
        }

        #endregion

        #region Validate

        public DbExecution<T> Validate(bool expression, string message)
        {
            if (!IsError && expression)
                Error = message;

            return this;
        }

        #endregion

        #region Execute

        public DbExecution<T> Execute(object data, ExecuteDelegate worker)
        {
            if (!IsError)
            {
                try
                {
                    Result = worker.Invoke(data, this);
                }
                catch(Exception ex)
                {
                    Error = ex;
                }
            }

            return this;
        }

        public DbExecution<T> Execute(Action<IExecution<T>> action)
        {
            if (!IsError)
            {
                try
                {
                    action.Invoke(this);
                }
                catch (Exception ex)
                {
                    Error = ex;
                }
            }
            
            return this;
        }

        #endregion
    }
}