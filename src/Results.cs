// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace UkrGuru.Sql;

public class Results
{
    public PropertyInfo[] Props { get; set; } = [];

    public int[] Indexes { get; set; } = [];

    public object[] Values { get; set; } = [];

    public static object? Parse(object? value) => value == DBNull.Value ? null : value;

    public static T? Parse<T>(object? value, T? defaultValue = default, JsonSerializerOptions? options = null)
    {
        Type returnType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (value is null || value is DBNull)
            return defaultValue;

        if (value is T t)
            return t;

        if (returnType == typeof(string))
            return (T?)(object?)Serialize(value, options) ?? defaultValue;

        if (value is string s)
            return (T?)Deserialize(s.AsSpan(), returnType, options);

        if (value is char[] chars)
            return (T?)Deserialize(chars.AsSpan(), returnType, options);

        if (returnType.IsEnum)
            return (T?)ParseEnum(returnType, Convert.ToString(value).AsSpan());

        if (value is DateTime dt && returnType == typeof(DateOnly))
            return (T?)(object?)DateOnly.FromDateTime(dt);

        if (value is TimeSpan ts && returnType == typeof(TimeOnly))
            return (T?)(object?)TimeOnly.FromTimeSpan(ts);

        return (T?)Convert.ChangeType(value, returnType);
    }

    private static string? Serialize(object? value, JsonSerializerOptions? options = null)
    {
        if (value is null || value is DBNull)
            return null;

        if (value is Enum e)
            return e.ToString();

        if (value is bool b1)
            return b1 ? "true" : "false";

        if (value is byte n1)
            return n1.ToString(CultureInfo.InvariantCulture);

        if (value is short n2)
            return n2.ToString(CultureInfo.InvariantCulture);

        if (value is int n3)
            return n3.ToString(CultureInfo.InvariantCulture);

        if (value is long n4)
            return n4.ToString(CultureInfo.InvariantCulture);

        if (value is float n5)
            return n5.ToString(CultureInfo.InvariantCulture);

        if (value is double n6)
            return n6.ToString(CultureInfo.InvariantCulture);

        if (value is decimal n7)
            return n7.ToString(CultureInfo.InvariantCulture);

        if (value is DateOnly d1)
            return d1.ToString("o", CultureInfo.InvariantCulture);

        if (value is DateTime d2)
            return d2.ToString("o", CultureInfo.InvariantCulture);

        if (value is DateTimeOffset d3)
            return d3.ToString("o", CultureInfo.InvariantCulture);

        if (value is TimeOnly t1)
            return t1.ToString("o", CultureInfo.InvariantCulture);

        if (value is TimeSpan t2)
            return t2.ToString("c", CultureInfo.InvariantCulture);

        if (value is Guid g)
            return g.ToString();

        if (value is char c)
            return c.ToString();

        if (value is string s)
            return s;

        if (value is byte[] ab)
            return Convert.ToBase64String(ab);

        if (value is char[] ac)
            return new string(ac);

        return JsonSerializer.Serialize(value, options);
    }

    private static object? Deserialize(ReadOnlySpan<char> value, Type type, JsonSerializerOptions? options = null)
    {
        if (type == typeof(string))
            return value.ToString();

        if (type == typeof(bool))
            return bool.Parse(value);

        if (type == typeof(byte))
            return byte.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(short))
            return short.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(int))
            return int.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(long))
            return long.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(float))
            return float.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(double))
            return double.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(decimal))
            return decimal.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(DateOnly))
            return DateOnly.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(DateTime))
            return DateTime.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(DateTimeOffset))
            return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(TimeOnly))
            return TimeOnly.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(TimeSpan))
            return TimeSpan.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(Guid))
            return Guid.Parse(value);

        if (type == typeof(char))
            return value[0];

        if (type == typeof(byte[]))
            return Convert.FromBase64String(value.ToString());

        if (type == typeof(char[]))
            return value.ToArray();

        if (type.IsEnum)
            return ParseEnum(type, value);

        return JsonSerializer.Deserialize(value, type, options);
    }

    private static object ParseEnum(Type t, ReadOnlySpan<char> value)
    {
        if (Enum.TryParse(t, value, ignoreCase: true, out object? result) && Enum.IsDefined(t, result)) 
            return result;

        throw new ArgumentException($"'{value}' is not a valid value for enum {t.Name}");
    }
}