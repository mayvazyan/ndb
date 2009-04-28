namespace ITCreatings.Ndb.Utils
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
                if (!IsChar(ch) && !IsDigit(ch) && ch != '_')
                    return false;
                
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified symbol is char.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>
        /// 	<c>true</c> if the specified symbol is char; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsChar(char symbol)
        {
            return (symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z');
        }

        /// <summary>
        /// Determines whether the specified symbol is digit.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>
        /// 	<c>true</c> if the specified symbol is digit; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDigit(char symbol)
        {
            return (symbol >= '0' && symbol <= '9');
        }
    }
}
