using System;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Invalid Column Name Exception
    /// </summary>
    [Serializable] 
    public class NdbInvalidColumnNameException : NdbException

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NdbInvalidColumnNameException"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        public NdbInvalidColumnNameException(string columnName) 
            : base("{0} is not valid column name", columnName)
        {
        }
    }
}