// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace UkrGuru.Sql
{
    public class Results
    {
        public PropertyInfo[] Props { get; set; } = Array.Empty<PropertyInfo>();

        public int[] Indexes { get; set; } = Array.Empty<int>();

        public object[] Values { get; set; } = Array.Empty<object>();

        private static readonly Dictionary<Type, Func<object, object?>> TypeParsers = new()
        {
            { typeof(byte[]), value => Convert.FromBase64String(value.ToString()!) },
            { typeof(char[]), value => value.ToString()!.ToCharArray() },
            { typeof(string), value => value.ToJson().Trim('"') },
            { typeof(char), value => value.ToString()![0] },
            { typeof(Guid), value => Guid.Parse(value.ToString()!) },
            { typeof(DateOnly), value => DateOnly.FromDateTime(Convert.ToDateTime(value)) },
            { typeof(DateTime), value => Convert.ToDateTime(value) },
            { typeof(DateTimeOffset), value => new DateTimeOffset(Convert.ToDateTime(value)) },
            { typeof(TimeOnly), value => TimeOnly.Parse(value.ToString()!) },
            { typeof(TimeSpan), value => TimeSpan.ParseExact(value.ToString()!, "c", null) }
        };

        public static object? Parse(object? value) => value == DBNull.Value ? default : value;

        public static T? Parse<T>(object? value, T? defaultValue = default) => value switch
        {
            null => defaultValue,
            DBNull => defaultValue,
            T t => (T?)t,
            Guid g => (T?)(object)g.ToString(),
            JsonElement je => je.ValueKind == JsonValueKind.Null ? defaultValue : ParseJE<T>(je),
            _ => Parse<T>(value)
        };

        private static T? Parse<T>(object value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
        {
            Type t when TypeParsers.TryGetValue(t, out var parser) => (T?)parser(value),
            Type t when t.IsEnum => (T)Enum.Parse(t, Convert.ToString(value)!),
            Type t when t.IsClass => JsonSerializer.Deserialize<T?>(value.ToString()!),
            Type t => (T?)Convert.ChangeType(value, t, CultureInfo.InvariantCulture)
        };

        private static T? ParseJE<T>(JsonElement value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
        {
            Type t when t == typeof(string) => Parse<T>(value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText().Trim('"')),
            _ => value.Deserialize<T>()
        };
    }
}