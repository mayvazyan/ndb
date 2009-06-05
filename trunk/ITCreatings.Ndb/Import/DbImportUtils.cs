using System;
using System.Text.RegularExpressions;

namespace ITCreatings.Ndb.Import
{
    /// <summary>
    /// Impoirt Utils methods
    /// </summary>
    public class DbImportUtils
    {
        /// <summary>
        /// Removes the extra spaces.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string RemoveExtraSpaces(string input)
        {
            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double GetValueOrDefault(double? value)
        {
            return value.HasValue ? value.Value : 0;
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static DateTime GetValueOrDefault(DateTime? value, DateTime defaultValue)
        {
            return value.HasValue ? value.Value : defaultValue;
        }
    }
}