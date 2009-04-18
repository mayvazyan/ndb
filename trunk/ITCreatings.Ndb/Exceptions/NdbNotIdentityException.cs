namespace ITCreatings.Ndb.Exceptions
{
    /// <summary>
    /// Occurs if we try to use PrimaryKey on records which hasn't identity field
    /// </summary>
    public class NdbNotIdentityException : NdbException
    {
        private const string MSG = "This method can be called only for Identity records, therefore the following error occured:\r\n";

        /// <summary>
        /// Allows to cpecify exception details
        /// </summary>
        /// <param name="message"></param>
        public NdbNotIdentityException(string message) : base(MSG + message)
        {
        }
    }
}
