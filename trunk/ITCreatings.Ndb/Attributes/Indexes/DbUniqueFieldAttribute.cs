namespace ITCreatings.Ndb.Attributes
{
    /// <summary>
    /// Marks an field as unique
    /// </summary>
    /// <example>
    /// <code>
    ///    [DbRecord("TasksAssignments")]
    ///    public class TaskAssignment
    ///    {
    ///        private const string IndexName = "TaskUser";
    ///
    ///        [DbForeignKeyField(typeof(Task))]
    ///        [DbUniqueField(IndexName = IndexName)]
    ///        public ulong TaskId;
    ///
    ///        [DbUniqueField(IndexName = IndexName)]
    ///        [DbForeignKeyField(typeof(User))]
    ///        public ulong UserId;
    ///    }
    /// </code></example>
    public class DbUniqueFieldAttribute : DbIndexedFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbUniqueFieldAttribute"/> class.
        /// </summary>
        public DbUniqueFieldAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbUniqueFieldAttribute"/> class.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        public DbUniqueFieldAttribute(string indexName)
        {
            IndexName = indexName;
        }
    }
}