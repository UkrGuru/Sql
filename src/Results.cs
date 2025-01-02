// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace UkrGuru.Sql;

public class Results
{
    public PropertyInfo[] Props { get; set; } = Array.Empty<PropertyInfo>();

    public int[] Indexes = Array.Empty<int>();

    public object[] Values = Array.Empty<object>();

    public static object? Parse(object? value) => value == DBNull.Value ? default : value;

    public static T? Parse<T>(object? value, T? defaultValue = default) => value switch
    {
        null => defaultValue,
        DBNull => defaultValue,
        T t => (T?)t,
        string s => ParseS<T>(s),
        Guid g => (T?)(object)g.ToString(),
        JsonElement je => je.ValueKind == JsonValueKind.Null ? defaultValue : ParseJE<T>(je),
        _ => Parse<T>(value)
    };

    static T? Parse<T>(object value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t.IsEnum => (T)Enum.Parse(t, Convert.ToString(value)!),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime((DateTime)value),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value),
        Type t when t == typeof(string) => (T)(object)value.ToJson().Trim('"'),
        _ => (T?)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
    }; 

    static T? ParseJE<T>(JsonElement value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t == typeof(string) => ParseS<T>(value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText().Trim('"')),
        _ => value.Deserialize<T>()
    };

    static T? ParseS<T>(string value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t == typeof(byte[]) => (T)(object)Convert.FromBase64String(value),
        Type t when t == typeof(char[]) => (T)(object)value.ToCharArray(),
        Type t when t == typeof(string) => (T)(object)value,
        Type t when t == typeof(char) => (T)(object)value[0],
        Type t when t.IsClass => JsonSerializer.Deserialize<T?>(value),
        Type t when t == typeof(Guid) => (T)(object)Guid.Parse(value),
        Type t when t.IsEnum => (T)Enum.Parse(t, value!),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime(Convert.ToDateTime(value)),
        Type t when t == typeof(DateTime) => (T)(object)Convert.ToDateTime(value),
        Type t when t == typeof(DateTimeOffset) => (T)(object)new DateTimeOffset(Convert.ToDateTime(value)),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.Parse(value),
        Type t when t == typeof(TimeSpan) => (T)(object)TimeSpan.ParseExact(value, "c", null),
        Type t => (T?)Convert.ChangeType(value, t, CultureInfo.InvariantCulture)
    };
}

//public T? ExecuteScalarToNullable<T>(string connectionString, string query) where T : struct
//{
//    using (SqlConnection connection = new SqlConnection(connectionString))
//    {
//        SqlCommand command = new SqlCommand(query, connection);
//        connection.Open();
//        object result = command.ExecuteScalar();
//        return ConvertResult<T>(result);
//    }
//}
