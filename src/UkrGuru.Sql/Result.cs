// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace UkrGuru.Sql;

/// <summary>
/// Represents a class for mapping SQL query results to C# objects.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets or sets an array of PropertyInfo objects representing the properties of the mapped object.
    /// </summary>
    public PropertyInfo[] Props { get; set; } = Array.Empty<PropertyInfo>();

    /// <summary>
    /// Gets or sets an array of nullable integers representing the column indexes from the SQL query result.
    /// </summary>
    public int[] Indexes = Array.Empty<int>();

    /// <summary>
    /// Gets or sets an array of objects representing the values retrieved from the SQL query result.
    /// </summary>
    public object[] Values = Array.Empty<object>();

    public static object? Parse(object? value) => value == DBNull.Value ? default : value;

    public static T? Parse<T>(object? value, T? defaultValue = default) =>
        value == null || value == DBNull.Value ? defaultValue :
        value is T t ? t :
        value is string s ? ParseS<T>(s) :
        value is JsonElement je ? je.ValueKind == JsonValueKind.Null ? defaultValue : ParseJE<T>(je) :
        Parse<T>(value);

    static T? Parse<T>(object value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t.IsEnum => (T)Enum.Parse(t, Convert.ToString(value)!),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime((DateTime)value),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value),
        Type t => (T?)Convert.ChangeType(value, t, CultureInfo.InvariantCulture)
    };

    static T? ParseJE<T>(JsonElement value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t == typeof(string) => ParseS<T>(value.ValueKind == JsonValueKind.String ? 
            value.GetString()! : value.GetRawText().Trim('"')),
        _ => value.Deserialize<T>()
    };

    static T? ParseS<T>(string value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t == typeof(char[]) => (T)(object)value.ToCharArray(),
        Type t when t == typeof(string) => (T)(object)value,
        Type t when t == typeof(char) => (T)(object)value[0],
        Type t when t.IsClass => JsonSerializer.Deserialize<T?>(value),
        Type t => (T?)Convert.ChangeType(value, t, CultureInfo.InvariantCulture),
    };
}