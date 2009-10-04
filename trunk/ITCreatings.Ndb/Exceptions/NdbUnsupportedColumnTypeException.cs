using System;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Unsupported Column Type exception. 
    /// </summary>
    [Serializable]
    public class NdbUnsupportedColumnTypeException : NdbException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NdbUnsupportedColumnTypeException"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="valueType">Type of the value.</param>
        public NdbUnsupportedColumnTypeException(DbProvider provider, Type valueType)
            : base("The {0} doesn't supports values of the {1} type", provider, valueType)
        {
        }
    }
}