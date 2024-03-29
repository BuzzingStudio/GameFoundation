﻿namespace GameFoundation.BlueprintFlow.BlueprintReader.CsvHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sylvan.Data.Csv;

    public static class CsvHelper
    {
        public static readonly  TypeConverterCache                 TypeConverterCache = new TypeConverterCache();
        private static readonly Dictionary<Type, List<MemberInfo>> MemberInfosCache   = new Dictionary<Type, List<MemberInfo>>();

        public static readonly CsvDataReaderOptions CsvDataReaderOptions = new CsvDataReaderOptions()
        {
            HasHeaders = true,
            Delimiter  = ','
        };

        public static string GetField(this CsvDataReader csvReader, string name)
        {
            try
            {
                return csvReader.GetString(csvReader.GetOrdinal(name));
            }
            catch (IndexOutOfRangeException e)
            {
                return string.Empty;
            }
        }

        public static T GetField<T>(this CsvDataReader csvReader, string name)
        {
            var index = csvReader.GetOrdinal(name);
            return (T)GetField(csvReader, typeof(T), index);
        }

        public static T GetField<T>(this CsvDataReader csvReader, int index) { return (T)GetField(csvReader, typeof(T), index); }

        public static object GetField(this CsvDataReader csvReader, Type type, int index)
        {
            var field     = csvReader.GetString(index);
            var converter = TypeConverterCache.GetConverter(type);
            return converter.ConvertFromString(field, type);
        }

        /// <summary>
        /// Utility to get all member infos from a class map
        /// </summary>
        public static List<MemberInfo> GetAllFieldAndProperties(this Type typeInfo)
        {
            if (MemberInfosCache.TryGetValue(typeInfo, out var results)) return results;

            results = typeInfo.GetFields().Select(fieldInfo => new MemberInfo()
                { MemberName = fieldInfo.Name, MemberType = fieldInfo.FieldType, SetValue = fieldInfo.SetValue, GetValue = fieldInfo.GetValue }).ToList();
            results.AddRange(typeInfo.GetProperties().Select(propertyInfo => new MemberInfo()
                { MemberName = propertyInfo.Name, MemberType = propertyInfo.PropertyType, SetValue = propertyInfo.SetValue, GetValue = propertyInfo.GetValue }));

            return results;
        }

        public class MemberInfo
        {
            public string                 MemberName;
            public Type                   MemberType;
            public Action<object, object> SetValue;
            public Func<object, object>   GetValue;
        }
    }
}