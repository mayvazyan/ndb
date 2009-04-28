using System;
using System.Diagnostics;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;
using log4net;

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
    /// DbExecutionError executionError = executor.Error;
    /// </code>
    /// </example>
    /// </summary>
    public class DbExecution<T> : IDbExecution<T>
    {
        #region Messages

        /// <summary>
        /// Default message
        /// </summary>
        public string InvalidArgumentsMessage = @"Invalid arguments";

        #endregion

        #region data

        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        public DbExecutionError Error 
        { 
            get { return error; }
            set
            {
                error = value;

                logError();
            }
        }

        private DbExecutionError error;
        
        /// <summary>
        /// Is an Error occured during validate
        /// </summary>
        public bool IsError { get { return Error != DbExecutionError.Empty; } }

        private readonly ILog logger;
        
        /// <summary>
        /// Logger
        /// </summary>
        public ILog Logger 
        { 
            get
            {
                if (logger == null)
                    throw new NdbException("Logger wasn't set");

                return logger;
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Execution delegate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        public delegate T ExecuteDelegate(object data, IDbExecution<T> execution);

        #endregion

        #region Factory

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <returns></returns>
        public static DbExecution<T> Create()
        {
            return new DbExecution<T>();
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="logger">logger can be accessed later as IDbExecution.Logger</param>
        /// <returns></returns>
        public static DbExecution<T> Create(ILog logger)
        {
            return new DbExecution<T>(logger);
        }

        private DbExecution()
        {
            Error = DbExecutionError.Empty;    
        }

        private DbExecution(ILog log) : this()
        {
            logger = log;
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
            return Validate(expression, InvalidArgumentsMessage);
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
            {
                Error = customErrorCode;
            }

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
            {
                Error = message;
            }

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
                logInfo("Delegate executing...");
                try
                {
                    Result = worker.Invoke(data, this);
                    logOk();
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
                logInfo("Anonymous delegate executing...");
                try
                {
                    action.Invoke(this);
                    logOk();
                }
                catch (Exception ex)
                {
                    Error = ex;
                }
            }
            
            return this;
        }

        #endregion

        #region Log methods

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> LogInfo(string message)
        {
            if (!IsError && logger != null)
                logger.Info(message);

            return this;
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> LogDebug(string message)
        {
            if (!IsError && logger != null)
                logger.Debug(message);

            return this;
        }

        #endregion

        #region utils

        private void logOk()
        {
            logInfo("Ok");
        }

        private void logInfo(string message)
        {
            if (logger != null)
                logger.Info(message);
        }

        private void logError()
        {
            if (logger != null)
            {
                if (error.IsException)
                {
                    logger.Error(error.Message, error.Exception);
                }
                else
                {
                    StackTrace stackTrace = new StackTrace();
                    logger.Error(string.Format("{0}. Stack Trace:\r\n{1}", error.Message, stackTrace));
                }
            }
        }

        #endregion
    }
}