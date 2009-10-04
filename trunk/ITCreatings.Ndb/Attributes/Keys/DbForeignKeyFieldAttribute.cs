using System;

namespace ITCreatings.Ndb.Attributes
{
    /// <summary>
    /// Marks Field as ForeignKey 
    /// </summary>
    public sealed class DbForeignKeyFieldAttribute : DbFieldAttribute
    {
        /// <summary>
        /// Returns ForeignKey Type
        /// </summary>
        public Type ForeignKeyType { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DbForeignKeyFieldAttribute"/> class.
        /// </summary>
        /// <param name="foreignKeyType">Type of the foreign key.</param>
        public DbForeignKeyFieldAttribute(Type foreignKeyType)
        {
            ForeignKeyType = foreignKeyType;
        }
    }
}