using System;

namespace ITCreatings.Ndb.Attributes
{
    /// <summary>
    /// Marks field as storable to database
    /// <example>
    /// <code>
    /// [DbField]
    /// public string LastName;
    /// 
    /// [DbField("CreationDate")]
    /// public DateTime Date;
    /// </code>
    /// </example>
    /// </summary>
    public class DbFieldAttribute : Attribute
    {
        /// <summary>
        /// Name of the associated column
        /// <example>
        /// <code>
        /// [DbField("CreationDate")]
        /// public DateTime Date;
        /// </code>
        /// </example>
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets or sets the column size.
        /// </summary>
        /// <value>The size.</value>
        internal uint Size { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the associated column</param>
        public DbFieldAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">The Size of the associated column</param>
        public DbFieldAttribute(uint size)
        {
            Size = size;
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="name">The Name of the associated column</param>
        /// <param name="size">The Size of the associated column</param>
        public DbFieldAttribute(string name, uint size) : this(name)
        {
            Size = size;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbFieldAttribute()
        {
        }
    }
}