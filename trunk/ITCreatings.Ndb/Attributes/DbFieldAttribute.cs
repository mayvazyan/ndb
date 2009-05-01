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
        public uint Size { get; private set; }



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the associated column</param>
        public DbFieldAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbFieldAttribute()
        {
        }
    }
}