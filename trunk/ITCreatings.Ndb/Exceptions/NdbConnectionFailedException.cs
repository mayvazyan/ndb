using System;

namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Informs about Connection Failed exceptions
    /// </summary>
    public class NdbConnectionFailedException : NdbException
    {
        internal NdbConnectionFailedException(string connectionString, Exception ex) :
            base("Can't connect to database: " + connectionString, ex) { }

        internal NdbConnectionFailedException(string connectionString) :
            base("Can't connect to database: " + connectionString) { }
    }
}
