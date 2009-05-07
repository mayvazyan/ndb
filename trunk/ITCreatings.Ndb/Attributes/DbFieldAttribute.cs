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
    /// 
    /// [DbField(1024)]
    /// public byte[] Data;
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
        /// Can be used to specify max size of the string and byte[] fields.
        /// Default value is 255.
        /// <example>
        /// <code>
        /// [DbField(1024)]
        /// public byte[] Data;
        /// 
        /// [DbField(512)]
        /// public string Data;
        /// </code>
        /// </example>
        /// </summary>
        /// <value>The size.</value>
        internal uint Size { get; private set; }

        /// <summary>
        /// Gets or sets the type of the db field.
        /// </summary>
        /// <value>The type of the db field.</value>
        internal Type DbType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [differs from database type].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [differs from database type]; otherwise, <c>false</c>.
        /// </value>
        internal bool DiffersFromDatabaseType { get { return DbType != null; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the associated column</param>
        public DbFieldAttribute(string name)
        {
            Name = name;
            Size = 255;
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
        /// Initializes a new instance of the <see cref="DbFieldAttribute"/> class.
        /// </summary>
        /// <param name="type">Database field type.</param>
        public DbFieldAttribute(Type type)
        {
            DbType = type;
            Size = 255;
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="columnName">The Name of the associated column</param>
        /// <param name="size">The Size of the associated column</param>
        public DbFieldAttribute(string columnName, uint size)
            : this(columnName)
        {
            Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFieldAttribute"/> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="dbType">Type of the db.</param>
        public DbFieldAttribute(string columnName, Type dbType)
            : this(columnName)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFieldAttribute"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="dbType">Type of the db.</param>
        public DbFieldAttribute(uint size, Type dbType)
            : this(size)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFieldAttribute"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="size">The size.</param>
        /// <param name="dbType">Type of the db.</param>
        public DbFieldAttribute(string columnName, uint size, Type dbType)
            : this(columnName, size)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbFieldAttribute()
        {
            Size = 255;
        }
    }
}