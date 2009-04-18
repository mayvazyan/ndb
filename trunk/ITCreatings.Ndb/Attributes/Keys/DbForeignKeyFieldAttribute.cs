using System;

namespace ITCreatings.Ndb.Attributes.Keys
{
    /// <summary>
    /// Marks Field as ForeignKey 
    /// </summary>
    public class DbForeignKeyFieldAttribute : DbFieldAttribute
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