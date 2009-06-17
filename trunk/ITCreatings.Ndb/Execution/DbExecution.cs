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
    /// var execution = DbExecution&lt;ExecutionResultCode&gt;.Create()
    ///         .IsTrue(user.Id != 0, message1)
    ///         .IsNullOrEmpty(user.Password, message2)
    ///         .SetPossibleResultCode(ExecutionResultCode.UnableUpdateData)
    ///         .Execute(exec => dbGateway.Update(user));
    /// 
    /// 
    /// ExecutionResultCode resultCode = execution.ResultCode;
    /// 
    /// ...
    /// 
    /// private enum ExecutionResultCode
    /// {
    ///     Success,
    /// 
    ///     UnableLoadData,
    ///     UnableUpdateData,
    ///     InvalidPasswordLength,
    /// 
    ///     ...
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class DbExecution<TResultCode> : DbExecution<TResultCode, TResultCode>
    {
        
    }

    /// <summary>
    /// Execution flow helper
    /// <example>
    /// <code>
    /// var execution = DbExecution&lt;User, ExecutionResultCode&gt;.Create()
    ///         .IsTrue(user.Id != 0, message1)
    ///         .IsNullOrEmpty(user.Password, message2)
    ///         .SetPossibleResultCode(ExecutionResultCode.UnableLoadData)
    ///         .Execute(exec => exec.Result = dbGateway.Load&lt;User&gt;(...));
    /// 
    /// ExecutionResultCode resultCode;
    /// User user = execution.GetResult(out resultCode); // resultCode will be set to ExecutionResultCode.UnableLoadData if an exception occured
    /// 
    /// ...
    /// 
    /// private enum ExecutionResultCode
    /// {
    ///     Success,
    /// 
    ///     UnableLoadData,
    ///     InvalidPasswordLength,
    /// 
    ///     ...
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class DbExecution<TResult, TResultCode> : IDbExecution<TResult, TResultCode>
    {
        #region Data

        /// <summary>
        /// Result
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        /// <value>The result code.</value>
        public TResultCode ResultCode { get { return Error.ResultCode; } }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <returns></returns>
        public TResult GetResult(out TResultCode resultCode)
        {
            resultCode = Error.ResultCode;
            return Result;
        }

        /// <summary>
        /// Gets or sets the possible result code.
        /// If an exception occurs during execution this value will be returned as Error.CustomResultCode
        /// </summary>
        /// <value>The result code.</value>
        public TResultCode PossibleResultCode { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        public DbExecutionError<TResultCode> Error
        {
            get { return error; }
            set
            {
                error = value;

                logError();
            }
        }

        private DbExecutionError<TResultCode> error;

        /// <summary>
        /// Is an Error occured during validate
        /// </summary>
        public bool IsError { get { return Error.IsError; } }

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

        #region Factory

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <returns></returns>
        public static DbExecution<TResult, TResultCode> Create()
        {
            return new DbExecution<TResult, TResultCode>();
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="logger">logger can be accessed later as IDbExecution.Logger</param>
        /// <returns></returns>
        public static DbExecution<TResult, TResultCode> Create(ILog logger)
        {
            return new DbExecution<TResult, TResultCode>(logger);
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="logger">The logger can be accessed later as IDbExecution.Logger</param>
        /// <param name="successResultCode">The no error.</param>
        /// <returns></returns>
        public static DbExecution<TResult, TResultCode> Create(ILog logger, TResultCode successResultCode)
        {
            return new DbExecution<TResult, TResultCode>(logger, successResultCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExecution&lt;TResult, TResultCode&gt;"/> class.
        /// </summary>
        protected DbExecution()
        {
            Error = default(TResultCode);    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExecution&lt;TResult, TResultCode&gt;"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        protected DbExecution(ILog log) : this()
        {
            logger = log;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExecution&lt;TResult, TResultCode&gt;"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="successResultCode">The no error.</param>
        protected DbExecution(ILog log, TResultCode successResultCode) : this()
        {
            logger = log;
            Error = successResultCode;
        }

        #endregion
        
        #region Messages

        /// <summary>
        /// Default message
        /// </summary>
        public string InvalidArgumentsMessage = @"Invalid arguments";

        #endregion

        #region Delegates

        /// <summary>
        /// Execution delegate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        public delegate void ExecuteDelegate(object data, IDbExecution<TResult, TResultCode> execution);

        #endregion

        #region Validation

        #region True&False

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsTrue(bool expression, object description)
        {
            if (!IsError && !expression)
            {
                if (description == null)
                    Error = InvalidArgumentsMessage;
                else
                if (description is TResultCode)
                    Error = (TResultCode)description;
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
        public DbExecution<TResult, TResultCode> IsTrue(bool expression)
        {
            return IsTrue(expression, null);
        }

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsFalse(bool expression)
        {
            return IsTrue(!expression);
        }

        /// <summary>
        /// Validates given expression to be a true
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsFalse(bool expression, object description)
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
        public DbExecution<TResult, TResultCode> IsMatchRegex(object value, string pattern, object message)
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
        public DbExecution<TResult, TResultCode> IsMatchRegex(object value, string pattern, object message, RegexOptions options)
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
        public DbExecution<TResult, TResultCode> IsNull(object value, object message)
        {
            bool isNull = DbRecordInfo.IsNull(value);
            return IsTrue(isNull, message);
        }
        
        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsNull(object value)
        {
            return IsNull(value, null);
        }

        /// <summary>
        /// Determines whether the specified value is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="description">The message.</param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsNotNull(object value, object description)
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
        public DbExecution<TResult, TResultCode> IsEmail(object value, object message)
        {
            const string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
         @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

//            const string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" +
//                                   @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" +
//                                   @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";

            return IsMatchRegex(value, pattern, message);
        }

        /// <summary>
        /// Determines whether the specified value is URL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsUrl(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsIp(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsPhone(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsSin(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsZero(object value, object description)
        {
            return IsFalse(0.Equals(value), description);
        }

        /// <summary>
        /// Determines whether the specified value is decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsDecimal(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsInt(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsLong(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsMoney(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsLength(object value, int length, object message)
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
        public DbExecution<TResult, TResultCode> IsStartWith(object value, string startChunk, object message)
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
        public DbExecution<TResult, TResultCode> IsEndsWith(object value, string endChunk, object message)
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
        public DbExecution<TResult, TResultCode> IsLongerThan(object value, int length, object message)
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
        public DbExecution<TResult, TResultCode> IsShorterThan(object value, int length, object message)
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
        public DbExecution<TResult, TResultCode> IsNotNullOrEmpty(object value, object message)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(value as string);
            return IsFalse(isNullOrEmpty, message);
        }

        #endregion

        #region DateTime

        
        /// <summary>
        /// Determines whether the specified value is date time
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> IsDateTime(object value, object message)
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
        public DbExecution<TResult, TResultCode> IsEarlierThan(object value, DateTime dateTime, object message)
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
        public DbExecution<TResult, TResultCode> IsLaterThan(object value, DateTime dateTime, object message)
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
        /// Sets the possible result code.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> SetPossibleResultCode(TResultCode resultCode)
        {
            PossibleResultCode = resultCode;
            return this;
        }


        /// <summary>
        /// Executes ExecuteDelegate delegate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="worker"></param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> Execute(object data, ExecuteDelegate worker)
        {
            if (!IsError)
            {
                logInfo("Delegate executing...");
                try
                {
                    worker.Invoke(data, this);
                    logOk();
                }
                catch(Exception ex)
                {
                    Error = new DbExecutionError<TResultCode>(ex, PossibleResultCode);
                }
            }

            return this;
        }

        /// <summary>
        /// Executes anonymous delegate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DbExecution<TResult, TResultCode> Execute(Action<IDbExecution<TResult, TResultCode>> action)
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
                    Error = new DbExecutionError<TResultCode>(ex, PossibleResultCode);
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
        public DbExecution<TResult, TResultCode> LogInfo(string message)
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
        public DbExecution<TResult, TResultCode> LogDebug(string message)
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
                    logger.Error(string.Format("{0}. Stack Trace:\r\n{1}", error.Message, new StackTrace()));
                }
            }
        }

        #endregion
    }
}