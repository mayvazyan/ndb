using System;
using System.Globalization;

namespace ITCreatings.Ndb.Utils
{
    /// <summary>
    /// Provides access to the string.Format method
    /// </summary>
    public static class DbString
    {
        /// <summary>
        /// string.Format wrapper
        /// </summary>
        /// <param name="stringFormat">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Format(string stringFormat, params object [] args)
        {
            return string.Format(CultureInfo.InvariantCulture, stringFormat, args);
        }

        /// <summary>
        /// Reports the index of the first occurrence
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int IndexOf(string source, string value)
        {
            return value.IndexOf(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
