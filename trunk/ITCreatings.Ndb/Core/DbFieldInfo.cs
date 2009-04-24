using System;
using System.Reflection;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Contains all required info about field
    /// </summary>
    public class DbFieldInfo
    {
        private readonly FieldInfo FieldInfo;

        /// <summary>
        /// Field Type
        /// </summary>
        public Type FieldType { get { return FieldInfo.FieldType; } }

        /// <summary>
        /// Name of the associated column
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates DbFieldInfo instance
        /// </summary>
        /// <param name="fieldInfo">Field Info</param>
        /// <param name="name">Name of the associated column</param>
        public DbFieldInfo(FieldInfo fieldInfo, string name)
        {
            FieldInfo = fieldInfo;
            Name = (string.IsNullOrEmpty(name)) ? fieldInfo.Name : name;
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
