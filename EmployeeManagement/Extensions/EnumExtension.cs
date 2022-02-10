using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EmployeeManagement.Extensions
{
    public static class EnumExtension
    {
        public static Dictionary<int, string> ToDictionary<TEnum>() where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum is not an Enum type");

            return Enum.GetValues(typeof(TEnum))
                .Cast<object>()
                .ToDictionary(item => (int)item, value => (value as Enum).ToString());
        }

        public static Dictionary<TEnum, string> ToDictionaryWithEnumAsKeyValue<TEnum>() where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum is not an Enum type");

            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToDictionary(item => item, value => (value as Enum).ToString());
        }

        public static Dictionary<TEnum, string> ToDictionaryWithDescriptionAsValue<TEnum>() where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum is not an Enum type");

            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToDictionary(item => item, value => (value as Enum).GetDescription());
        }

        public static Dictionary<string, string> ToDictionaryWithDescriptionAsKeyValue<TEnum>() where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum is not an Enum type");

            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToDictionary(item => (item as Enum).GetDescription(), value => (value as Enum).GetDescription());
        }

        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }
    }
}
