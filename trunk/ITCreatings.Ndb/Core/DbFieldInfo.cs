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
        private readonly MemberInfo FieldInfo;

        /// <summary>
        /// Gets or sets the Field Type
        /// </summary>
        public Type FieldType { get { return DbFieldInfo.GetType(FieldInfo); } }

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
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue { get; private set; }

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
        /// <param name="defaultValue">The default value.</param>
        public DbFieldInfo(MemberInfo fieldInfo, string name, uint size, Type dbType, object defaultValue)
        {
            FieldInfo = fieldInfo;
            Name = (string.IsNullOrEmpty(name)) ? fieldInfo.Name : name;
            Size = size;
            DbType = dbType;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets value of the field
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="obj">Source object</param>
        /// <returns>Current value</returns>
        public static object GetValue(MemberInfo memberInfo, object obj)
        {
            if (memberInfo is PropertyInfo)
            {
                return ((PropertyInfo)memberInfo).GetValue(obj, null);
            }

            return ((FieldInfo)memberInfo).GetValue(obj);
        }

        /// <summary>
        /// Gets value of the field
        /// </summary>
        /// <param name="obj">Source object</param>
        /// <returns>Current value</returns>
        public object GetValue(object obj)
        {
            return GetValue(FieldInfo, obj);
        }

        /// <summary>
        /// Gets value of the field
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="data">Target object</param>
        /// <param name="value">New value</param>
        public static void SetValue(MemberInfo memberInfo, object data, object value)
        {
            if (memberInfo is PropertyInfo)
            {
                ((PropertyInfo)memberInfo).SetValue(data, value, null);
            }
            else
                ((FieldInfo)memberInfo).SetValue(data, value);
        }

        /// <summary>
        /// Gets value of the field
        /// </summary>
        /// <param name="data">Target object</param>
        /// <param name="value">New value</param>
        public void SetValue(object data, object value)
        {
            SetValue(FieldInfo,data,value);
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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(@"DbFieldInfo(""{0}"", {1})", Name, FieldType);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
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
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (Name + Size + FieldType).GetHashCode();
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static Type GetType(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                return ((PropertyInfo)memberInfo).PropertyType;
            }
            return ((FieldInfo)memberInfo).FieldType;
        }
    }
}
