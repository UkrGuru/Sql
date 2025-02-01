// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace UkrGuru.Sql
{
    public class Results
    {
        public PropertyInfo[] Props { get; set; } = [];

        public int[] Indexes { get; set; } = [];

        public object[] Values { get; set; } = [];

        public static object? Parse(object? value) => value == DBNull.Value ? default : value;

        public static T? Parse<T>(object? value, T? defaultValue = default) => value switch
        {
            null => defaultValue,
            DBNull => defaultValue,
            T t => t,
            char[] chars => (T)(new string(chars) as object),
            JsonElement je => je.ValueKind == JsonValueKind.Null ? defaultValue : ParseJE<T>(je),
            _ => Parse<T>(value)
        };

        private static T? ParseJE<T>(JsonElement value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
        {
            Type t when t == typeof(string) => (T)(value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText().Trim('"') as object),
            _ => value.Deserialize<T>()
        };

        private static T? Parse<T>(object value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
        {
            Type t when TypeParsers.TryGetValue(t, out var parser) => (T?)parser(value),
            Type t when t.IsEnum => (T)Enum.Parse(t, value.ToString()!),
            Type t => JsonSerializer.Deserialize<T>((string)value)
        };

        private static readonly Dictionary<Type, Func<object, object>> TypeParsers = new()
        {
            { typeof(byte[]), value => Convert.FromBase64String((string)value) },
            { typeof(char[]), value => ((string)value).ToCharArray() },
            { typeof(char), value => ((string)value)[0] },
            { typeof(string), value => value.ToJson().Trim('"') },
            { typeof(Guid), value => Guid.Parse((string)value) },
            { typeof(DateOnly), value => DateOnly.FromDateTime(Convert.ToDateTime(value)) },
            { typeof(DateTime), value => Convert.ToDateTime(value) },
            { typeof(DateTimeOffset), value => new DateTimeOffset(Convert.ToDateTime(value)) },
            { typeof(TimeOnly), value => TimeOnly.Parse(value.ToString()!) },
            { typeof(TimeSpan), value => TimeSpan.ParseExact(value.ToString()!, "c", null) },
        };
    }
}