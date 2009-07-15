using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ITCreatings.Ndb.Core
{
    internal class DbFieldCheckResult
    {
        private readonly DbAccessor accessor; 
        
        public bool IsNew { get; private set; }
        public bool IsDifferent { get; private set; }
        
        /// <summary>
        /// sql type for field
        /// </summary>
        public string SqlType { get; private set; }

        /// <summary>
        /// current sql type (from db)
        /// </summary>
        public string CurrentSqlType { get; private set; }

        public DbFieldCheckResult(DbAccessor accessor)
        {
            this.accessor = accessor;
        }

        public void Process(Dictionary<string, string> fields, DbFieldInfo fi)
        {
            SqlType = accessor.GetSqlType(fi);

            string columnName = (accessor.IsPostgre) ? fi.Name.ToLower() : fi.Name;

            if (!fields.ContainsKey(columnName))
                IsNew = true;
            else
            {
                CurrentSqlType = fields[columnName];

                if (CurrentSqlType != SqlType)
                {
                    string type1 = GetSqlType(CurrentSqlType);
                    string type2 = GetSqlType(SqlType);
                    if (type1 == type2
                        || (IsStringType(type1) && IsStringType(type1))
                        || (IsDateTimeType(type1) && IsDateTimeType(type1))
                        )
                        return;
                    
                    IsDifferent = true;
                }
                    
            }
        }
        
        private static string GetSqlType(string type)
        {
            int index = type.IndexOf('(');
            return index > 0 ? type.Substring(0, index) : type;
        }

        private static bool IsStringType(string type)
        {
            string[] stringTypes = new[] {"varchar", "nvarchar", "ntext", "text", "TINYTEXT", "LONGTEXT", "MEDIUMTEXT", "nchar"};
            return Array.IndexOf(stringTypes, type) > -1;
        }

        private static bool IsDateTimeType(string type)
        {
            string[] stringTypes = new[] {"datetime", "timestamp", "date", "time"};
            return Array.IndexOf(stringTypes, type) > -1;
        }
    }
}