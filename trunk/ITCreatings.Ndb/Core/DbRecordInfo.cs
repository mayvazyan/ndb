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
        public Type RecordType { get; set; }

        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Fields
        /// </summary>
        public DbFieldInfo[] Fields { get; set; }

        /// <summary>
        /// Childs
        /// </summary>
        public Dictionary<Type, MemberInfo> Childs { get; set; }

        /// <summary>
        /// Parents
        /// </summary>
        public Dictionary<Type, MemberInfo> Parents { get; set; }

        /// <summary>
        /// Foreign Keys
        /// </summary>
        public Dictionary<Type, DbFieldInfo> ForeignKeys { get; set; }

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

                args[j++] = DbConverter.SetValue(Fields[i], value);
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