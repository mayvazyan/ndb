using System;
using System.Diagnostics;
using System.Reflection;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Contains all required info about field
    /// </summary>
    [DebuggerDisplay("DbFieldInfo ({Name}, {FieldType})")]
    public class DbFieldInfo
    {
        private readonly FieldInfo FieldInfo;

        /// <summary>
        /// Gets or sets the Field Type
        /// </summary>
        public Type FieldType { get { return FieldInfo.FieldType; } }

        /// <summary>
        /// Gets or sets the associated column Name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the associated column size.
        /// </summary>
        /// <value>The size.</value>
        public uint Size { get; private set; }

        /// <summary>
        /// Creates DbFieldInfo instance
        /// </summary>
        /// <param name="fieldInfo">Field Info</param>
        /// <param name="name">Name of the associated column</param>
        /// <param name="size">Size of the associated column</param>
        public DbFieldInfo(FieldInfo fieldInfo, string name, uint size)
        {
            FieldInfo = fieldInfo;
            Name = (string.IsNullOrEmpty(name)) ? fieldInfo.Name : name;
            Size = size;
        }

        /// <summary>
        /// Gets value of the field
        /// </summary>
        /// <param name="obj">Source object</param>
        /// <returns>Current value</returns>
        public object GetValue(object obj)
        {
            return FieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Gets value of the field
        /// </summary>
        /// <param name="data">Target object</param>
        /// <param name="value">New value</param>
        public void SetValue(object data, object value)
        {
            FieldInfo.SetValue(data, value);
        }

        /// <summary>
        /// Gets Custom Attributes
        /// </summary>
        /// <returns>Custom Attributes</returns>
        public object[] GetCustomAttributes()
        {
            return FieldInfo.GetCustomAttributes(true);
        }
    }
}
