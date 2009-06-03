using System;
using System.Text;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Utils
{
    internal class DbConvertor
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
            if (fieldType == typeof(Guid))//if db engine doesn't support GUID we just apply this "patch" for it:)
            {
                if (value is string)
                    return new Guid((string) value);

                if (value is byte[])
                    return new Guid((byte[])value);
            }

            return value;
        }

        internal static object convertData(Type targetType, object value)
        {
            if (targetType == typeof(byte[]) && value is string)
                return Encoding.UTF8.GetBytes(value as string);

            if (targetType == typeof(byte[]) && value is Int32)
                return BitConverter.GetBytes((Int32)value);

            if (targetType == typeof(string) && value is byte[])
                return Encoding.UTF8.GetString(value as byte[]);

            if (targetType == typeof(Int32) && value is byte[])
                return BitConverter.ToInt32(value as byte[], 0);

            if (targetType == typeof(Int32) && value is string)
            {
                Int32 ival;
                return Int32.TryParse(value.ToString(), out ival) ? ival : 0;
            }

            if (targetType == typeof(Int64) && value is string)
            {
                Int32 ival;
                return Int32.TryParse(value.ToString(), out ival) ? ival : 0;
            }

            if (targetType == typeof(double))
            {
                Double dval;
                return Double.TryParse(value.ToString(), out dval) ? dval : 0;
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
