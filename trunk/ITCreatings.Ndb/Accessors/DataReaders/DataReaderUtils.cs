using System;
using System.Collections.Generic;
using System.Data;

namespace ITCreatings.Ndb.Accessors.DataReaders
{
    internal class DataReaderUtils
    {
        internal static List<string> ReadNames(IDataReader reader)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object value = reader[i];
                if (value != DBNull.Value)
                    names.Add(value.ToString());
            }
            return names;
        }

        internal static int GetOrdinal(List<string> names, string name)
        {
            int indexOf = names.IndexOf(name);
            if (indexOf == -1)
                throw new Exception(string.Format("Field {0} wasn't found", name));

            return indexOf;
        }
    }
}