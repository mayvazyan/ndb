using System;

namespace ITCreatings.Ndb.Attributes
{
    /// <summary>
    /// Marks Field as ForeignKey 
    /// </summary>
    public sealed class DbForeignKeyFieldAttribute : DbFieldAttribute
    {
        /// <summary>
        /// Return ForeignKey Type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// ForeignKey Type should be specifyed
        /// </summary>
        /// <param name="type">ForeignKey Type</param>
        public DbForeignKeyFieldAttribute(Type type)
        {
            Type = type;
        }
    }
}