using System;
using System.Collections.Generic;

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
            Type fieldType = fi.FieldType;
            SqlType = accessor.GetSqlType(fieldType);

            string columnName = (accessor.IsPostgre) ? fi.Name.ToLower() : fi.Name;

            if (!fields.ContainsKey(columnName))
                IsNew = true;
            else
            {
                CurrentSqlType = fields[columnName];

                if (accessor.IsMySql)
                {
                    if (fieldType.BaseType == typeof (Enum))
                        fieldType = Enum.GetUnderlyingType(fieldType);

                    if (fieldType != accessor.GetType(CurrentSqlType))
                        IsDifferent = true;
                }
                else
                {
                    if (CurrentSqlType != accessor.GetSqlType(fieldType))
                        IsDifferent = true;
                }
            }
        }
    }
}