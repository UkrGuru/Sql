
using System.Text.Json;
using System.Text;
using System.Globalization;

namespace UkrGuru.Sql;

public class Scalar
{
    /// <summary>
    /// Converts the object value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The sql object value to convert.</param>
    /// <param name="defaultValue">The default value to return if the conversion fails.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? Parse<T>(object? value, T? defaultValue = default) =>
        value == null || value == DBNull.Value || value == Array.Empty<T>() ? defaultValue :
        value is T t ? t :
        value is string s ? (s.Length > 0 ? ParseS<T>(s) : defaultValue) :
        value is StringBuilder sb ? (sb.Length > 0 ? ParseS<T>(sb.ToString()) : defaultValue) :
        value is Guid g ? ParseS<T>(g.ToString()) :
        value is JsonElement je ? je.ValueKind == JsonValueKind.Null ? defaultValue : ParseJE<T>(je) :
        Parse<T>(value);

    /// <summary>
    /// Converts an object to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="value">The object to convert.</param>
    /// <returns>The converted object.</returns>
    static T? Parse<T>(object value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        //Type t when t == typeof(Guid) => (T)(object)Guid.Parse(Convert.ToString(value)!),
        Type t when t.IsEnum => (T)Enum.Parse(t, Convert.ToString(value)!),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime((DateTime)value),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value),
        Type t => (T?)Convert.ChangeType(value, t),
    };

    static T? ParseJE<T>(JsonElement value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t == typeof(string) => value.ValueKind == JsonValueKind.String ? 
            (T)(object)value.GetString()! : ParseS<T>(value.GetRawText().Trim('"')),
        _ => value.Deserialize<T>(),
    };

    /// <summary>
    /// Converts a string to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="value">The string to convert.</param>
    /// <returns>The converted object.</returns>
    static T? ParseS<T>(string value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t == typeof(byte[]) => (T)(object)Encoding.UTF8.GetBytes(value),
        Type t when t == typeof(char[]) => (T)(object)value.ToCharArray(),
        Type t when t == typeof(string) => (T)(object)value,
        Type t when t == typeof(char) => (T)(object)value[0],
        Type t when t.IsClass => JsonSerializer.Deserialize<T?>(value),
        Type t when t == typeof(Guid) => (T)(object)Guid.Parse(value),
        Type t when t.IsEnum => (T)Enum.Parse(t, value),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime(Convert.ToDateTime(value)),
        Type t when t == typeof(DateTime) => (T)(object)Convert.ToDateTime(value),
        Type t when t == typeof(DateTimeOffset) => (T)(object)new DateTimeOffset(Convert.ToDateTime(value)),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.Parse(value),
        Type t when t == typeof(TimeSpan) => (T)(object)TimeSpan.ParseExact(value, "c", null),
        Type t => (T?)Convert.ChangeType(value, t, CultureInfo.InvariantCulture),
    };
}