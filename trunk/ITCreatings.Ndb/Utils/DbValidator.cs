﻿namespace ITCreatings.Ndb.Utils
{
    /// <summary>
    /// Column Name Validator helper
    /// </summary>
    public static class DbValidator
    {
        /// <summary>
        /// Determines whether is valid column name
        /// </summary>
        /// <param name="ColumnName">Name of the column.</param>
        /// <returns>
        /// 	<c>true</c> if is valid column name; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidColumnName(string ColumnName)
        {
            if (string.IsNullOrEmpty(ColumnName))
                return false;

            for (int i = 0; i < ColumnName.Length; i++)
            {
                char ch = ColumnName[i];
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                    return false;
                
            }
            return true;
        }
    }
}
