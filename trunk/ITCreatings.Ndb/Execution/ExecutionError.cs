using System;

namespace ITCreatings.Ndb.Execution
{
    ///<summary>
    /// Execution Error holder
    ///</summary>
    public class ExecutionError
    {
        /// <summary>
        /// Underlayed Exception
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// Underlayed Error Code
        /// </summary>
        public ExecutionErrorCode ErrorCode;

        private string message;
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message
        {
            get
            {
                return (Exception != null) 
                    ? Exception.Message
                    : ((ErrorCode == ExecutionErrorCode.Custom) ? message : ErrorCode.ToString());
            }
            set { message = value; }
        }

        /// <summary>
        /// Returns ExecutionErrorCode as int
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator int(ExecutionError d)
        {
            return (int) d.ErrorCode;
        }

        /// <summary>
        /// Returns string represantation of contained error
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator string(ExecutionError d)
        {
            return d.Message;
        }
        
        /// <summary>
        /// Creates ExecutionError instance from string message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static implicit operator ExecutionError(string message)
        {
            return new ExecutionError
                       {
                           Message = message
                       };
        }

        /// <summary>
        /// Creates ExecutionError instance from an Exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static implicit operator ExecutionError(Exception exception)
        {
            ExecutionError error = new ExecutionError { Exception = exception };

            if (exception is Exceptions.NdbConnectionFailedException)
                error.ErrorCode = ExecutionErrorCode.ConnectionFailed;

            return error;
        }
    }
}