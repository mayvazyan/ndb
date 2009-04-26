using System;
using ITCreatings.Ndb.Query;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Invalid Filter Expression Exception
    /// </summary>
    public class DbInvalidFilterException : NdbException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="expressionType"></param>
        /// <param name="type"></param>
        public DbInvalidFilterException(DbExpressionType expressionType, Type type) 
            : base("{0} not supported by {1}", expressionType, type)
        {
        }
    }
}
