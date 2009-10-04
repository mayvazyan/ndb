using System;
using ITCreatings.Ndb.Query;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Invalid Filter Expression Exception
    /// </summary>
    [Serializable]
    public class NdbInvalidFilterException : NdbException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="expressionType"></param>
        /// <param name="type"></param>
        public NdbInvalidFilterException(DbExpressionType expressionType, Type type) 
            : base("{0} not supported by {1}", expressionType, type)
        {
        }
    }
}
