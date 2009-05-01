using System;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Invalid Column Size Exception
    /// </summary>
    public class NdbInvalidColumnSizeException : NdbException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NdbInvalidColumnSizeException"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="size">The size.</param>
        public NdbInvalidColumnSizeException(string type, uint size)
            : base("The {0} type cannot be a {0} size", type, size)
        {
        }
    }
}