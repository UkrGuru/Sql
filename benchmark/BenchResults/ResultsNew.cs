// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text.Json;

namespace UkrGuru.Sql;

public class ResultsNew
{
    public static object? Parse(object? value) => value == DBNull.Value ? null : value;

    public static T? Parse<T>(object? value, T? defaultValue = default, JsonSerializerOptions? options = null) => value switch
    {
        null => defaultValue,
        DBNull => defaultValue,
        T t => t,
        string s => typeof(T) switch
        {
            Type t when TypeDeserializers.TryGetValue(t, out var deserializer) => (T?)deserializer(s),
            Type t when t.IsEnum => (T?)SerilizeEnum(t, s),
            _ => JsonSerializer.Deserialize<T?>(s, options),
        },
        char[] chars => Parse<T>(new string(chars), defaultValue, options),
        JsonElement je => je.ValueKind switch
        {
            JsonValueKind.Null => defaultValue,
            _ => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
            {
                Type t when t == typeof(string) => (T?)(object?)(je.ValueKind switch
                {
                    JsonValueKind.String => je.GetString()!,
                    _ => je.GetRawText().Trim('"')
                }),
                _ => je.Deserialize<T?>(options)
            }
        },
        _ => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
        {
            Type t when t == typeof(string) => (T?)(TypeSerializers.TryGetValue(value.GetType(), out var serialize) switch
            {
                true => serialize(value),
                _ => (object?)JsonSerializer.Serialize(value, options),
            }),
            Type t when TypeParsers.TryGetValue(t, out var parser) => (T?)parser(value),
            Type t when t.IsEnum => (T?)SerilizeEnum(t, value),
            _ => JsonSerializer.Deserialize<T?>((string)value, options),
        }
    };

    private static object SerilizeEnum(Type t, object value) => Enum.TryParse(t, Convert.ToString(value), out object? result) switch
    {
        true when Enum.IsDefined(t, result) => result,
        _ => throw new ArgumentException($"'{value}' is not a valid value for enum {t.Name}")
    };

    private static readonly Dictionary<Type, Func<string, object?>> TypeDeserializers = new()
    {
        { typeof(bool), value => bool.Parse(value) },
        { typeof(byte), value => byte.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(short), value => short.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(int), value => int.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(long), value => long.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(float), value => float.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(double), value => double.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(decimal), value => decimal.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(DateOnly), value => DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture) },
        { typeof(DateTime), value => DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) },
        { typeof(DateTimeOffset), value => DateTimeOffset.ParseExact(value, "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", CultureInfo.InvariantCulture) },
        { typeof(TimeOnly), value => TimeOnly.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture) },
        { typeof(TimeSpan), value => TimeSpan.ParseExact(value, "c", CultureInfo.InvariantCulture) },
        { typeof(Guid), value => Guid.Parse(value) },
        { typeof(char), value => char.Parse(value) },
        { typeof(string), value => value },
        { typeof(byte[]), Convert.FromBase64String },
        { typeof(char[]), value => value.ToCharArray() }
    };

    private static readonly Dictionary<Type, Func<object, object>> TypeParsers = new()
    {
        { typeof(bool), value => Convert.ToBoolean(value) },
        { typeof(byte), value => Convert.ToByte(value, CultureInfo.InvariantCulture) },
        { typeof(short), value => Convert.ToInt16(value, CultureInfo.InvariantCulture) },
        { typeof(int), value => Convert.ToInt32(value, CultureInfo.InvariantCulture) },
        { typeof(long), value => Convert.ToInt64(value, CultureInfo.InvariantCulture) },
        { typeof(float), value => Convert.ToSingle(value, CultureInfo.InvariantCulture) },
        { typeof(double), value => Convert.ToDouble(value, CultureInfo.InvariantCulture) },
        { typeof(decimal), value => Convert.ToDecimal(value, CultureInfo.InvariantCulture) },
        { typeof(DateOnly), value => DateOnly.FromDateTime(Convert.ToDateTime(value)) },
        { typeof(DateTime), value => Convert.ToDateTime(value) },
        { typeof(DateTimeOffset), value => new DateTimeOffset(Convert.ToDateTime(value)) },
        { typeof(TimeSpan), value => TimeSpan.ParseExact(Convert.ToString(value)!, "c", null) },
        { typeof(TimeOnly), value => TimeOnly.Parse(Convert.ToString(value)!) },
        //{ typeof(Guid), data => Convert.ToGuid(data) },
        { typeof(char), value => Convert.ToChar(value) },
        { typeof(string), data => Convert.ToString(data)! },
        { typeof(byte[]), value => Convert.FromBase64String(Convert.ToString(value)!) },
        { typeof(char[]), value => Convert.ToString(value)!.ToCharArray() },
    };

    private static readonly Dictionary<Type, Func<object, string?>> TypeSerializers = new()
    {
        { typeof(bool), data => (bool)data ? "true" : "false" },
        { typeof(byte), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(short), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(int), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(long), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(float), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(double), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(decimal), data => Convert.ToString(data, CultureInfo.InvariantCulture) },
        { typeof(DateOnly), data => ((DateOnly)data).ToString("yyyy-MM-dd") },
        { typeof(DateTime), data => Convert.ToDateTime(data).ToString("yyyy-MM-ddTHH:mm:ss.fff") },
        { typeof(DateTimeOffset), data => ((DateTimeOffset)data).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'") },
        { typeof(TimeOnly), data => ((TimeOnly)data).ToString("HH:mm:ss", CultureInfo.InvariantCulture) },
        { typeof(TimeSpan), data => ((TimeSpan)data).ToString("c") },
        { typeof(Guid), data => Convert.ToString(data) },
        { typeof(char), data => Convert.ToString(data) },
        { typeof(string), data => Convert.ToString(data) },
        { typeof(byte[]), data => Convert.ToBase64String((byte[])data) },
        { typeof(char[]), data => new string((char[])data) }
    };
}