namespace ITCreatings.Ndb.Attributes
{
    /// <summary>
    /// Marks field as index
    /// </summary>
    /// <example>
    /// <code>
    /// [DbRecord]
    /// public class User
    /// {
    ///     private const string IndexName = "Name";
    ///
    ///     [DbIndexedField(IndexName = IndexName)]
    ///     public string FirstName;
    ///
    ///     [DbIndexedField(IndexName = IndexName)]
    ///     public string LastName;
    /// }
    /// </code></example>

    public class DbIndexedFieldAttribute : DbFieldAttribute
    {
        /// <summary>
        /// Name of the index. 
        /// Should be specifyed for the Indexes which contains several columns
        /// </summary>
        public string IndexName { get; protected set; }
    }
}