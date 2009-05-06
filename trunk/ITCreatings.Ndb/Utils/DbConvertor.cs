﻿using System;
using System.Text;
using ITCreatings.Ndb.Core;

namespace ITCreatings.Ndb.Utils
{
    internal class DbConvertor
    {
        public static object SetValue(DbFieldInfo field, object value)
        {
            return (field.IsDiffersFromDatabaseType)
                       ? convertData(field.DbType, value)
                       : value;
        }

        public static object GetValue(DbFieldInfo field, object value)
        {
            return (field.IsDiffersFromDatabaseType)
                               ? convertData(field.FieldType, value)
                               : fixData(field.FieldType, value);

        }

        private static object fixData(Type fieldType, object value)
        {
            if (fieldType == typeof(Guid) && value is string) //mysql doesn't support GUID so this is just a "patch" for it:)
                return new Guid((string)value);

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

            return Convert.ChangeType(value, targetType);
        }
    }
}