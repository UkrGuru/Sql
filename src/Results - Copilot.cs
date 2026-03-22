using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace UkrGuru.Sql;

public class Results
{
    public PropertyInfo[] Props { get; set; } = Array.Empty<PropertyInfo>();
    public int[] Indexes { get; set; } = Array.Empty<int>();
    public object[] Values { get; set; } = Array.Empty<object>();

    private static readonly CultureInfo CI = CultureInfo.InvariantCulture;

    // -------------------------------------------------------------
    // NULL HANDLING
    // -------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? Parse(object? value)
        => value is DBNull ? null : value;

    // -------------------------------------------------------------
    // MAIN PARSE<T> — supports SQL values AND string primitives
    // -------------------------------------------------------------
    public static T? Parse<T>(object? value, T? defaultValue = default)
    {
        if (value is null || value is DBNull) return defaultValue;
        if (value is T exact) return exact;

        Type target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        // Fast-span extraction
        ReadOnlySpan<char> span =
            value is string s1 ? s1.AsSpan() :
            value is char[] ca ? ca.AsSpan() :
            value.ToString().AsSpan();

        // ---- BOOL ----
        if (target == typeof(bool))
            return (T)(object)FastBool(span);

        // ---- CHAR ----
        if (target == typeof(char))
            return (T)(object)span[0];

        // ---- BYTE ----
        if (target == typeof(byte))
            return (T)(object)byte.Parse(span, CI);

        // ---- SHORT ----
        if (target == typeof(short))
            return (T)(object)short.Parse(span, CI);

        // ---- INT ----
        if (target == typeof(int))
            return (T)(object)int.Parse(span, CI);

        // ---- LONG ----
        if (target == typeof(long))
            return (T)(object)long.Parse(span, CI);

        // ---- FLOAT ----
        if (target == typeof(float))
            return (T)(object)float.Parse(span, CI);

        // ---- DOUBLE ----
        if (target == typeof(double))
            return (T)(object)double.Parse(span, CI);

        // ---- DECIMAL ----
        if (target == typeof(decimal))
            return (T)(object)decimal.Parse(span, CI);

        // ---- GUID ----
        if (target == typeof(Guid) || target == typeof(Guid?))
            return (T)(object)Guid.Parse(span);

        // ---- CHAR[] ----
        if (target == typeof(char[]))
            return (T)(object)span.ToArray();

        // ---- STRING target ----
        if (target == typeof(string))
            return (T)(object)Serialize(value);

        // -------------------------------------------------------------
        // STRING CONVERSIONS FOR DATE/TIME TYPES
        // -------------------------------------------------------------
        string str = span.ToString();

        // DateOnly — support both direct and DateTime formats
        if (target == typeof(DateOnly))
        {
            if (DateTime.TryParse(str, out var dt2))
                return (T)(object)DateOnly.FromDateTime(dt2);

            return (T)(object)DateOnly.Parse(str, CI);
        }

        if (target == typeof(TimeOnly))
            return (T)(object)TimeOnly.Parse(str, CI);

        if (target == typeof(TimeSpan))
            return (T)(object)TimeSpan.Parse(str, CI);

        if (target == typeof(DateTimeOffset))
            return (T)(object)DateTimeOffset.Parse(str, CI, DateTimeStyles.RoundtripKind);

        if (target == typeof(DateTime))
            return (T)(object)DateTime.Parse(str, CI);

        // ---- byte[] (Base64) ----
        if (target == typeof(byte[]))
        {
            if (value is byte[] bb) return (T)(object)bb;
            return (T)(object)Convert.FromBase64String(str);
        }

        // ---- ENUM ----
        if (target.IsEnum)
            return (T)Enum.Parse(target, str, ignoreCase: true);

        // ---- JSON OBJECT FALLBACK ----
        if (!IsSimpleValueType(target) && LooksLikeJson(str))
            return (T?)JsonSerializer.Deserialize(str, target);

        // ---- FINAL FALLBACK ----
        return (T)Convert.ChangeType(str, target, CI);
    }

    // -------------------------------------------------------------
    // BOOL fast-path
    // -------------------------------------------------------------
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FastBool(ReadOnlySpan<char> span)
    {
        // Lowercase expected: "true"/"false"
        if (span.Length == 4 && (span[0] == 't' || span[0] == 'T')) return true;
        if (span.Length == 5 && (span[0] == 'f' || span[0] == 'F')) return false;
        return bool.Parse(span);
    }

    // -------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------
    private static bool LooksLikeJson(string s)
        => s.TrimStart().StartsWith("{") || s.TrimStart().StartsWith("[");

    private static bool IsSimpleValueType(Type t)
    {
        if (t.IsPrimitive) return true;

        return
            t == typeof(string) ||
            t == typeof(DateTime) ||
            t == typeof(DateOnly) ||
            t == typeof(TimeOnly) ||
            t == typeof(DateTimeOffset) ||
            t == typeof(TimeSpan) ||
            t == typeof(decimal) ||
            t == typeof(Guid) ||
            t == typeof(char) ||
            t == typeof(char[]) ||
            t == typeof(byte[]) ||
            t == typeof(int) ||
            t == typeof(long) ||
            t == typeof(double) ||
            t == typeof(float) ||
            t == typeof(short) ||
            t == typeof(byte) ||
            t == typeof(bool);
    }

    // -------------------------------------------------------------
    // OLD SERIALIZATION RULES (your tests depend on exact format)
    // -------------------------------------------------------------
    private static string Serialize(object value)
    {
        return value switch
        {
            bool b => b ? "true" : "false",
            byte bt => bt.ToString(CI),
            short s => s.ToString(CI),
            int i => i.ToString(CI),
            long l => l.ToString(CI),
            float f => f.ToString(CI),
            double d => d.ToString(CI),
            decimal m => m.ToString(CI),

            DateOnly d1 => d1.ToString("yyyy-MM-dd", CI),
            DateTime dt => dt.ToString("o", CI),
            DateTimeOffset dto => dto.ToString("o", CI),
            TimeOnly to => to.ToString("o", CI),
            TimeSpan ts => ts.ToString("c", CI),

            Guid g => g.ToString(),

            byte[] ba => Convert.ToBase64String(ba),
            char[] ca => new string(ca),
            char c => c.ToString(),

            _ => value.ToString() ?? string.Empty
        };
    }
}