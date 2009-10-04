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
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DbFieldAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Name of the associated column
        /// <example>
        /// <code>
        /// [DbField("CreationDate")]
        /// public DateTime Date;
        /// </code>
        /// </example>
        /// </summary>
        public string ColumnName { get; private set; }

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
        public uint Size { get; private set; }

        /// <summary>
        /// Gets or sets the type of the db field.
        /// </summary>
        /// <value>The type of the db field.</value>
        public Type DbType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether field type differs from the database column type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if field type differs from database column type otherwise, <c>false</c>.
        /// </value>
        public bool IsDiffersFromDatabaseType { get { return DbType != null; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnName">Name of the associated column</param>
        public DbFieldAttribute(string columnName)
        {
            ColumnName = columnName;
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
        /// <param name="dbType">Database field type.</param>
        public DbFieldAttribute(Type dbType)
        {
            DbType = dbType;
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