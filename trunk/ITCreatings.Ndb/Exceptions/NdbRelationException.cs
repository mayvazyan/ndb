using System;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Invalid Relation Exception
    /// </summary>
    public class NdbRelationException : NdbException
    {
        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="message"></param>
        public NdbRelationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Allows to set message using "format interface"
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public NdbRelationException(string format, params object[] args) : base(format, args)
        {
        }

        /// <summary>
        /// Extended Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NdbRelationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}