using System;

namespace ITCreatings.Ndb.Execution
{
    ///<summary>
    /// Execution Error holder
    ///</summary>
    public class DbExecutionError
    {
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
        public bool IsCustomError { get { return CustomErrorCode > 0; } }

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
                if (IsCustomError)
                    return "Custom Error: " + CustomErrorCode;

                if (Exception != null)
                    return Exception.Message;

                return (ErrorCode == DbExecutionErrorCode.Custom) ? message : ErrorCode.ToString();
            }
            private set { message = value; }
        }

        #endregion

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