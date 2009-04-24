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
    /// [DbField(Name = "CreationDate")]
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
        /// [DbField(Name = "CreationDate")]
        /// public DateTime Date;
        /// </code>
        /// </example>
        /// </summary>
        public string Name { get; set; }
    }
}