using System;
using System.Text;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Utils
{
    internal class DbConverter
    {
        public static object SetValue(DbFieldInfo field, object value)
        {
            return field.IsDiffersFromDatabaseType
                       ? convertData(field.DbType, value)
                       : value;
        }

        public static object GetValue(DbFieldInfo field, object value)
        {
            Type type = GetType(field.FieldType);
            return field.IsDiffersFromDatabaseType
                               ? convertData(type, value)
                               : fixData(type, value);
        }

        private static object fixData(Type fieldType, object value)
        {
            if (fieldType == typeof(Guid)) //if db engine doesn't support GUID we just apply this "patch" for it
            {
                string strValue = value as string;
                if (strValue != null)
                    return new Guid(strValue);

                byte[] bytes = value as byte[];
                if (bytes != null)
                    return new Guid(bytes);
            }

            return value;
        }

        internal static object convertData(Type targetType, object value)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                if (string.IsNullOrEmpty(stringValue) 
                    || Equals(value, "NULL")) //fix for CSV files
                    return DBNull.Value;

                if (targetType == typeof (byte[]))
                    return Encoding.UTF8.GetBytes(stringValue);
            }

            byte[] bytes = value as byte[];
            if (bytes != null)
            {
                if (targetType == typeof(string))
                    return Encoding.UTF8.GetString(bytes);

                if (targetType == typeof(Int32))
                    return BitConverter.ToInt32(bytes, 0);
            }

            if (targetType == typeof(byte[]) && value is Int32)
                return BitConverter.GetBytes((Int32)value);
            
            if (targetType == typeof(Int32))
            {
                return Convert.ToInt32(value);
//                Int32 ival;
//                return Int32.TryParse(value.ToString(), out ival) ? ival : 0;
            }

            if (targetType == typeof(Int64))
            {
                return Convert.ToInt64(value);
//                Int32 ival;
//                return Int32.TryParse(value.ToString(), out ival) ? ival : 0;
            }

            if (targetType == typeof(double))
            {
                return Convert.ToDouble(value);
//                Double dval;
//                return Double.TryParse(value.ToString(), out dval) ? dval : 0;
            }
            
            return Convert.ChangeType(value, targetType);
        }

        internal static Type GetType(Type fieldType)
        {
            return (fieldType.BaseType != null && fieldType.BaseType == typeof(Enum))
                        ? Enum.GetUnderlyingType(fieldType)
                        : Nullable.GetUnderlyingType(fieldType) ?? fieldType;
        }
    }
}
