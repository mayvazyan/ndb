using System;
using System.Data.Common;
using System.Globalization;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Base Exception class
    /// </summary>
    [Serializable] 
    public class NdbException : DbException
    {
        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="message"></param>
        public NdbException(string message) : base(message)
        {
        }

        /// <summary>
        /// Allows to set message using "format interface"
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public NdbException(string format, params object [] args) : this(string.Format(CultureInfo.InvariantCulture,format, args))
        {   
        }

        /// <summary>
        /// Extended Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NdbException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Returns Error Message
        /// </summary>
        public override string Message
        {
            get
            {
                if (InnerException == null)
                    return base.Message;

                Exception ex = base.GetBaseException();

                return string.Format("{0}\r\nDetails:\r\n{1}", base.Message, ex.Message);
            }
        }
    }
}
