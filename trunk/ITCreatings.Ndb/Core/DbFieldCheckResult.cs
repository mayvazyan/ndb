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
                    Match m1 = DbCodeGenerator.ParseSqlType(CurrentSqlType);
                    Match m2 = DbCodeGenerator.ParseSqlType(SqlType);
                    if (m1.Success)
                    {
                        string type = (m2.Success) ? m2.Groups[1].Value : SqlType;
                        if (m1.Groups[1].Value == type)
                            return;
                    }
                    
                    IsDifferent = true;
                }
                    
            }
        }
    }
}