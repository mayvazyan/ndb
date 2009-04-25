using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ITCreatings.Ndb.Core
{
    /// <summary>
    /// Internal class. Contains all required information about mapped object
    /// </summary>
    [DebuggerDisplay("Table={TableName} Type={RecordType}")]
    internal class DbRecordInfo
    {
        public Type RecordType;
        public string TableName;
        public DbFieldInfo[] Fields;
        public Dictionary<Type, DbFieldInfo> ForeignKeys;

        /// <summary>
        /// returns Key-Value collection
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public object [] GetValues(object data, params object[] argsToAppend)
        {
            object[] args = new object[Fields.Length * 2 + argsToAppend.Length];

            int j = 0;
            for (int i = 0; i < Fields.Length; i++)
            {
                args[j++] = Fields[i].Name;
                args[j++] = Fields[i].GetValue(data);
            }
            argsToAppend.CopyTo(args, j);

            return args;
        }
        

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