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
        /// Gets or sets the type of the db field.
        /// </summary>
        /// <value>The type of the db field.</value>
        internal Type DbType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [differs from database type].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [differs from database type]; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDiffersFromDatabaseType { get { return DbType != null; } }

        /// <summary>
        /// Creates DbFieldInfo instance
        /// </summary>
        /// <param name="fieldInfo">Field Info</param>
        /// <param name="name">Name of the associated column</param>
        /// <param name="size">Size of the associated column</param>
        /// <param name="dbType">Type of the db field.</param>
        public DbFieldInfo(FieldInfo fieldInfo, string name, uint size, Type dbType)
        {
            FieldInfo = fieldInfo;
            Name = (string.IsNullOrEmpty(name)) ? fieldInfo.Name : name;
            Size = size;
            DbType = dbType;
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

        /// <summary>
        /// Returns a <see cref="TResult:System.String"/> that represents the current <see cref="TResult:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TResult:System.String"/> that represents the current <see cref="TResult:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(@"DbFieldInfo(""{0}"", {1})", Name, FieldType);
        }

        /// <summary>
        /// Determines whether the specified <see cref="TResult:System.Object"/> is equal to the current <see cref="TResult:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="TResult:System.Object"/> to compare with the current <see cref="TResult:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="TResult:System.Object"/> is equal to the current <see cref="TResult:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="TResult:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            DbFieldInfo info = obj as DbFieldInfo;
            return info != null && FieldType == info.FieldType && Name == info.Name && Size == info.Size;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="TResult:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (Name + Size + FieldType).GetHashCode();
        }
    }
}
