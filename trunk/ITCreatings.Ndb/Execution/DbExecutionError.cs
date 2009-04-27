using System;

namespace ITCreatings.Ndb.Execution
{
    ///<summary>
    /// Execution Error holder
    ///</summary>
    public class DbExecutionError
    {
        #region Messages

        public const string NO_ERRORS_MESSAGE = @"No errors";
        public const string NDB_ERROR_CODE_MESSAGE = @"Ndb Error Code: ";
        public const string CUSTOM_ERROR_CODE_MESSAGE = @"Custom Error Code: ";

        #endregion


        #region Conditions Helpers

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
        public bool IsCustomError { get { return CustomErrorCode >= 0; } }

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
        public int CustomErrorCode { get; private set; }

        private string message;
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message
        {
            get
            {
                if (ErrorCode != DbExecutionErrorCode.Custom)
                    return NDB_ERROR_CODE_MESSAGE + ErrorCode;

                if (message != null)
                    return message;

                if (Exception != null)
                    return Exception.Message;

                if (IsCustomError)
                    return CUSTOM_ERROR_CODE_MESSAGE + CustomErrorCode;

                return NO_ERRORS_MESSAGE;
            }
            private set { message = value; }
        }

        #endregion

        /// <summary>
        /// Empty error
        /// </summary>
        public static readonly DbExecutionError Empty = new DbExecutionError();

        /// <summary>
        /// Default constructor
        /// </summary>
        private DbExecutionError()
        {
            CustomErrorCode = -1;
        }

        #region Convertors

        /// <summary>
        /// Returns ExecutionErrorCode as int
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator int(DbExecutionError d)
        {
            return (int) d.ErrorCode;
        }

        /// <summary>
        /// Returns string represantation of contained error
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator string(DbExecutionError d)
        {
            return d.Message;
        }
        
        /// <summary>
        /// Creates DbExecutionError instance from string message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError(string message)
        {
            return new DbExecutionError
                       {
                           Message = message
                       };
        }

        /// <summary>
        /// Creates DbExecutionError instance from CustomErrorCode
        /// </summary>
        /// <param name="customErrorCode"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError(int customErrorCode)
        {
            return new DbExecutionError
                       {
                           CustomErrorCode = customErrorCode
                       };
        }

        /// <summary>
        /// Creates DbExecutionError instance from Custom Error Codes enum
        /// </summary>
        /// <param name="customErrorCode"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError(Enum customErrorCode)
        {
            return new DbExecutionError
                       {
                           CustomErrorCode = (int) (object) customErrorCode
                       };
        }

        /// <summary>
        /// Creates DbExecutionError instance from an Exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static implicit operator DbExecutionError(Exception exception)
        {
            DbExecutionError error = new DbExecutionError { Exception = exception };

            if (exception is Exceptions.NdbConnectionFailedException)
                error.ErrorCode = DbExecutionErrorCode.ConnectionFailed;

            return error;
        }

        #endregion
    }
}