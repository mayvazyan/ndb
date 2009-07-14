using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ITCreatings.Ndb.Accessors;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb
{
    /// <summary>
    /// Code Generator allows you to generate C# classes from database tables
    /// </summary>
    public class DbCodeGenerator
    {
        /// <summary>
        /// Namespace for generated classes
        /// </summary>
        public string Namespace = "ITCreatings.GeneratedObjects";
        private readonly DbStructureGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCodeGenerator"/> class.
        /// </summary>
        public DbCodeGenerator(DbStructureGateway gateway)
        {
            this.gateway = gateway;
        }

        /// <summary>
        /// Generates the class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public string GenerateClass(string tableName)
        {
            return GenerateClass(tableName, tableName);
        }

        /// <summary>
        /// Generates the class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public string GenerateClass(string tableName, string className)
        {
            string fields = buildFields(tableName);

            return
                string.Format(@"using System;
using ITCreatings.Ndb.ActiveRecord;
using ITCreatings.Ndb.Attributes;

namespace {0}
{{
    [DbRecord(""{1}"")]
    public class {2} : DbActiveRecord
    {{
{3}
    }}
}}",
                    Namespace, tableName, className, fields
                    );
        }

        private string buildFields(string tableName)
        {
            Dictionary<string, string> fields = gateway.Accessor.LoadFields(tableName);

            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string> field in fields)
            {
                string name = field.Key;
                uint size = 0;

                string type = getType(field.Value, ref size).ToString();
                if (type.StartsWith("System."))
                    type = type.Replace("System.", "");

                string attribute = (size > 0) ? string.Format(@"DbField({0})", size) : "DbField";
                sb.AppendFormat("        [{0}] public {1} {2};\r\n", attribute, type, name);
            }
            return sb.ToString();
/*            
        [DbPrimaryKeyField] public ulong Id;
        [DbField] public uint TaskId;
        [DbForeignKeyField(typeof(User))] public ulong UserId;
        [DbField] public uint SpentMinutes;
        [DbField] public DateTime Date;
        [DbField] public string Description;
        [DbField] public DateTime Timestamp;*/
        }


        /// <summary>
        /// Gets the type from the text representation of the SQL type (for any provider)
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        internal static Type getType(string desc, ref uint size)
        {
            if (desc.IndexOf("BLOB") >= 0)
            {
                size = MySqlAccessor.LONG_MINSIZE; //todo: change this
                return typeof(Byte[]);
            }

            if (desc.IndexOf("text") >= 0)
            {
                size = MySqlAccessor.LONG_MINSIZE; //todo: change this
                return typeof(string);
            }

            if (desc.StartsWith("int") || desc.StartsWith("mediumint"))
                return desc.EndsWith("unsigned") ? typeof(UInt32) : typeof(Int32);

            if (desc.StartsWith("bigint"))
                return (desc.EndsWith("unsigned") ? typeof(UInt64) : typeof(Int64));

            if (desc.StartsWith("smallint"))
                return (desc.EndsWith("unsigned") ? typeof(UInt16) : typeof(Int16));

            if (desc.StartsWith("tinyint"))
                return (desc.EndsWith("unsigned") ? typeof(Byte) : typeof(Byte));

            if (desc == "datetime" || desc == "date" || desc == "timestamp")
                return typeof(DateTime);

            if (desc == "datetime" || desc == "date" || desc == "timestamp")
                return typeof(DateTime);
            
            if (desc.StartsWith("float"))
                return (desc.EndsWith("unsigned") ? typeof(Double) : typeof(Double));

            Match match = ParseSqlType(desc);

            if (match.Success)
            {
                string sqltype = match.Groups[1].Value;
                string len = match.Groups[2].Value;

                if (sqltype == "varchar" || sqltype == "char")
                {
                    size = uint.Parse(len);
                    return typeof(string);
                }
            }

            throw new NdbException("can't find .NET type for the " + desc);
        }

        internal static Match ParseSqlType(string desc)
        {
            Regex regex = new Regex(@"([a-zA-Z_0-9]+)\(([-0-9]+)\)");
            return regex.Match(desc);
        }

    }
}
