using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
    ///         .IsTrue(user.Id != 0, message1)
    ///         .IsNullOrEmpty(user.Password, message2)
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

        #region Validation

        #region True&False

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public DbExecution<T> IsTrue(bool expression, object description)
        {
            if (!IsError && !expression)
            {
                if (description == null)
                    Error = InvalidArgumentsMessage;
                else
                if (description is int)
                    Error = (int)description;    
                else
                    Error = description.ToString();
            }

            return this;
        }

        /// <summary>
        /// Determines whether the specified expression is true.
        /// </summary>
        /// <param name="expression">if set to <c>true</c> [expression].</param>
        /// <returns></returns>
        public DbExecution<T> IsTrue(bool expression)
        {
            return IsTrue(expression, null);
        }

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DbExecution<T> IsFalse(bool expression)
        {
            return IsTrue(!expression);
        }

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public DbExecution<T> IsFalse(bool expression, object description)
        {
            return IsTrue(!expression, description);
        }

        #endregion

        #region Core

        /// <summary>
        /// Determines whether the specified value is matches regex.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsMatchRegex(object value, string pattern, object message)
        {
            return IsMatchRegex(value, pattern, message, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Validates the regex.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public DbExecution<T> IsMatchRegex(object value, string pattern, object message, RegexOptions options)
        {
            if (value == null)
                return IsTrue(false, message);

            Match match = Regex.Match(value.ToString().Trim(), pattern, options);
            return IsTrue(match.Success, message);
        }

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsNull(object value, object message)
        {
            bool isNull = DbRecordInfo.IsNull(value);
            return IsTrue(isNull, message);
        }
        
        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public DbExecution<T> IsNull(object value)
        {
            return IsNull(value, null);
        }

        /// <summary>
        /// Determines whether the specified value is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="description">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsNotNull(object value, object description)
        {
            bool isNull = DbRecordInfo.IsNull(value);
            return IsFalse(isNull, description);
        }

        #endregion

        #region Internet related

        /// <summary>
        /// Determines whether the specified value is email.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsEmail(object value, object message)
        {
            const string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" +
                                   @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" +
                                   @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";

            return IsMatchRegex(value, pattern, message);
        }

        /// <summary>
        /// Determines whether the specified value is URL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsUrl(object value, object message)
        {
            const string pattern = @"^\w+://(?:[\w-]+(?:\:[\w-]+)?\@)?(?:[\w-]+\.)+[\w-]+(?:\:\d+)?[\w- ./?%&=\+]*$";
            return IsMatchRegex(value, pattern, message);
        }

        /// <summary>
        /// Determines whether the specified value is ip.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsIp(object value, object message)
        {
            const string pattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";
            return IsMatchRegex(value, pattern, message);
        }

        #endregion

        #region Social

        /// <summary>
        /// Determines whether the specified value is  phone. ex. (800) 888-1211
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsPhone(object value, object message)
        {
            const string pattern = @"\(\d\d\d\) \d\d\d\-\d\d\d\d";
            return IsMatchRegex(value, pattern, message);
        }

        /// <summary>
        /// Determines whether the specified value is SIN. ex. 011-01-0111
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsSin(object value, object message)
        {
            const string pattern = @"[0-9]{3}-[0-9]{2}-[0-9]{4}";
            return IsMatchRegex(value, pattern, message);
        }

        #endregion

        #region Digits

        /// <summary>
        /// Determines whether the specified value is zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public DbExecution<T> IsZero(object value, object description)
        {
            return IsFalse(0.Equals(value), description);
        }

        /// <summary>
        /// Determines whether the specified value is decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsDecimal(object value, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            decimal result;
            return IsTrue(decimal.TryParse(value.ToString(), out result));
        }

        /// <summary>
        /// Determines whether the specified value is int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsInt(object value, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            int result;
            return IsTrue(int.TryParse(value.ToString(), out result));
        }

        /// <summary>
        /// Determines whether the specified value is long.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsLong(object value, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            long result;
            return IsTrue(long.TryParse(value.ToString(), out result));
        }
        
        /// <summary>
        /// Determines whether the specified value is money. ex. $1,990,999.99
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsMoney(object value, object message)
        {
            const string pattern = @"\$(((\d{1,3},)+\d{3})|\d+)\.\d{2}";
            return IsMatchRegex(value, pattern, message);
        }

        #endregion

        #region Strings

        /// <summary>
        /// Determines whether the specified string has passed length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsLength(object value, int length, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            return IsTrue(value.ToString().Length == length, message);

        }

        /// <summary>
        /// Determines whether the specified value is start with.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startChunk">The start chunk.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsStartWith(object value, string startChunk, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            return IsTrue(value.ToString().StartsWith(startChunk), message);

        }

        /// <summary>
        /// Determines whether the specified value ends with.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="endChunk">The end chunk.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsEndsWith(object value, string endChunk, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            return IsTrue(value.ToString().EndsWith(endChunk), message);

        }

        /// <summary>
        /// Determines whether the specified value is longer than.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsLongerThan(object value, int length, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            return IsTrue(value.ToString().Length > length, message);

        }

        /// <summary>
        /// Determines whether the specified value is shorter than length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsShorterThan(object value, int length, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            return IsTrue(value.ToString().Length < length, message);
        }

        /// <summary>
        /// Validates the given expression to be a Null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsNotNullOrEmpty(object value, object message)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(value as string);
            return IsTrue(isNullOrEmpty, message);
        }

        #endregion

        #region DateTime

        
        /// <summary>
        /// Determines whether the specified value is date time
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsDateTime(object value, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            DateTime dateTime;
            bool isParsed = DateTime.TryParse(value.ToString(), out dateTime);
            return IsTrue(isParsed, message);
        }

        /// <summary>
        /// Determines whether the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsEarlierThan(object value, DateTime dateTime, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            DateTime date;
            bool isParsed = DateTime.TryParse(value.ToString(), out date);
            return IsTrue(isParsed && date < dateTime, message);
        }

        /// <summary>
        /// Determines whether the specified value is later than.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<T> IsLaterThan(object value, DateTime dateTime, object message)
        {
            if (value == null)
                return IsTrue(false, message);

            DateTime date;
            bool isParsed = DateTime.TryParse(value.ToString(), out date);
            return IsTrue(isParsed && date > dateTime, message);
        }

        #endregion

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