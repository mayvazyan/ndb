using System;

namespace ITCreatings.Ndb.Execution
{
    ///<summary>
    /// Execution Error holder
    ///</summary>
    public class DbExecutionError<TExecutionResultCode>
    {
        #region Messages

        #endregion

        #region Conditions Helpers

        /// <summary>
        /// Gets a value indicating whether this instance is error.
        /// </summary>
        /// <value><c>true</c> if this instance is error; otherwise, <c>false</c>.</value>
        public bool IsError { get { return IsException || IsCustomResultCode || IsTextMessageError; } }

        /// <summary>
        /// Exception occured
        /// </summary>
        public bool IsException { get { return Exception != null; } }

        /// <summary>
        /// Ndb Exception Code was set
        /// </summary>
        public bool IsNdbException { get { return ErrorCode != DbExecutionErrorCode.Custom; } }

        /// <summary>
        /// Custom Error Code was set
        /// </summary>
        public bool IsCustomResultCode { get { return !default(TExecutionResultCode).Equals(ResultCode); } }

        /// <summary>
        /// string message  was set
        /// </summary>
        public bool IsTextMessageError { get { return message != null; } }

        #endregion

        #region Data containers

        /// <summary>
        /// Underlayed Exception
        /// </summary>
        public Exception Exception { get; private set;}

        /// <summary>
        /// Underlayed Error Code
        /// </summary>
        public DbExecutionErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Custom Error Code
        /// </summary>
        public TExecutionResultCode ResultCode { get; set; }

        private string message;
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message
        {
            get
            {
                if (IsCustomResultCode)
                    return DbExecutionErrorMessage.CUSTOM_ERROR_CODE_MESSAGE + ResultCode;

                if (message != null)
                    return message;

                if (ErrorCode != DbExecutionErrorCode.Custom)
                    return DbExecutionErrorMessage.NDB_ERROR_CODE_MESSAGE + ErrorCode;

                if (Exception != null)
                    return Exception.Message;

                return DbExecutionErrorMessage.NO_ERRORS_MESSAGE;
            }
            private set { message = value; }
        }

        #endregion

//        /// <summary>
//        /// Empty error
//        /// </summary>
//        public static readonly DbExecutionError<int> Empty = new DbExecutionError<int>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbExecutionError()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExecutionError&lt;TExecutionResultCode&gt;"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public DbExecutionError(Exception exception)
        {
            Exception = exception;

            if (exception is Exceptions.NdbConnectionFailedException)
                ErrorCode = DbExecutionErrorCode.ConnectionFailed;

            //TODO: add other types of exception
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExecutionError&lt;TExecutionResultCode&gt;"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="resultCode">The result code.</param>
        public DbExecutionError(Exception exception, TExecutionResultCode resultCode) : this(exception)
        {
            ResultCode = resultCode;
        }

        #region Convertors

        /// <summary>
        /// Returns ExecutionErrorCode as int
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator int(DbExecutionError<TExecutionResultCode> d)
        {
            return (int)d.ErrorCode;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ITCreatings.Ndb.Execution.DbExecutionError&lt;TExecutionResultCode&gt;"/>
        /// to TExecutionResultCode.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TExecutionResultCode(DbExecutionError<TExecutionResultCode> d)
        {
            return d.ResultCode;
        }

        /// <summary>
        /// Returns string represantation of contained error
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator string(DbExecutionError<TExecutionResultCode> d)
        {
            return d.Message;
        }
        
        /// <summary>
        /// Creates DbExecutionError instance from string message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError<TExecutionResultCode>(string message)
        {
            return new DbExecutionError<TExecutionResultCode>
                       {
                           Message = message
                       };
        }

        /// <summary>
        /// Creates DbExecutionError instance from ResultCode
        /// </summary>
        /// <param name="executionResultCode"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError<TExecutionResultCode>(TExecutionResultCode executionResultCode)
        {
            return new DbExecutionError<TExecutionResultCode>
                       {
                           ResultCode = executionResultCode
                       };
        }

        /// <summary>
        /// Creates DbExecutionError instance from an Exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError<TExecutionResultCode>(Exception exception)
        {
            return new DbExecutionError<TExecutionResultCode>(exception);
        }

        #endregion
    }
}