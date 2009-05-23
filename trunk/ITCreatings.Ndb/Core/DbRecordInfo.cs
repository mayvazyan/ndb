using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ITCreatings.Ndb.Utils;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Internal class. Contains all required information about mapped object
    /// </summary>
    [DebuggerDisplay("Table={TableName} Type={RecordType}")]
    public class DbRecordInfo
    {
        /// <summary>
        /// Record Type
        /// </summary>
        public Type RecordType;

        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName;

        /// <summary>
        /// Fields
        /// </summary>
        public DbFieldInfo[] Fields;

        /// <summary>
        /// Childs
        /// </summary>
        public Dictionary<Type, FieldInfo> Childs;

        /// <summary>
        /// Parents
        /// </summary>
        public Dictionary<Type, FieldInfo> Parents;

        /// <summary>
        /// Foreign Keys
        /// </summary>
        public Dictionary<Type, DbFieldInfo> ForeignKeys;

        /// <summary>
        /// returns Key-Value collection
        /// </summary>
        /// <param name="data"></param>
        /// <param name="argsToAppend"></param>
        /// <returns></returns>
        public object [] GetValues(object data, params object[] argsToAppend)
        {
            var args = new object[Fields.Length * 2 + argsToAppend.Length];

            int j = 0;
            for (int i = 0; i < Fields.Length; i++)
            {
                args[j++] = Fields[i].Name;
                object value = Fields[i].GetValue(data);

                args[j++] = DbConvertor.SetValue(Fields[i], value);
            }
            argsToAppend.CopyTo(args, j);

            return args;
        }

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNull(object value)
        {
            return value == null
                       //strange thing but primaryKey.Equals(0) doesn't working for ulong type
                       || value.ToString() == "0"
                       || value.Equals(String.Empty)
                       || value.Equals(Guid.Empty);
        }
    }
}