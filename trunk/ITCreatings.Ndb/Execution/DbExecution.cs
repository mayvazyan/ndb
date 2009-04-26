using System;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Execution
{
    /// <summary>
    /// Execution flow helper
    /// <example>
    /// <code>
    /// var executor = DbExecution{User}.Create()
    ///         .Validate(user.Id == 0, message1)
    ///         .Validate(string.IsNullOrEmpty(user.Password), message2)
    ///         .Execute(delegate (IDbExecution{User} execution) { execution.Error = "Test"; });
    /// 
    /// bool isError = executor.IsError;
    /// User user = executor.Result;
    /// DbExecutionError executionError = executor.DbExecutionError;
    /// </code>
    /// </example>
    /// </summary>
    public class DbExecution<T> : IDbExecution<T>
    {
        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        public DbExecutionError Error { get; set;}
        
        /// <summary>
        /// Is an Error occured during validate
        /// </summary>
        public bool IsError { get { return Error != null; } }
        
        /// <summary>
        /// Execution delegate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        public delegate T ExecuteDelegate(object data, IDbExecution<T> execution);

        #region Factory

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <returns></returns>
        public static DbExecution<T> Create()
        {
            return new DbExecution<T>();
        }

        #endregion

        #region Validate

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DbExecution<T> Validate(bool expression)
        {
            return Validate(expression, "Invalid arguments");
        }

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="customErrorCode"></param>
        /// <returns></returns>
        public DbExecution<T> Validate(bool expression, int customErrorCode)
        {
            if (!IsError && expression)
                Error = customErrorCode;

            return this;
        }

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DbExecution<T> Validate(bool expression, string message)
        {
            if (!IsError && expression)
                Error = message;

            return this;
        }

        /// <summary>
        /// Validates given expression to be a Null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbExecution<T> ValidateIsNull(object value)
        {
            bool isNull = DbRecordInfo.IsNull(value);
            return Validate(isNull);
        }

        /// <summary>
        /// Validates given expression to be a Null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DbExecution<T> ValidateIsNull(object value, string message)
        {
            bool isNull = DbRecordInfo.IsNull(value);
            return Validate(isNull, message);
        }

        #endregion

        #region Execute

        /// <summary>
        /// Executes ExecuteDelegate delegate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="worker"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Executes anonymous delegate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DbExecution<T> Execute(Action<IDbExecution<T>> action)
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