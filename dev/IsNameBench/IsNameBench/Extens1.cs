//✅ 1.Your old UkrGuru.Sql.Extens.IsName is near - optimal
//At ~171 ns, your V1 implementation is already:

//allocation - free ✅
//branch - efficient ✅
//JIT - friendly ✅
//ASCII - based ✅
//structurally simple ✅

//This is exactly the profile of code that the .NET JIT optimizes best.
//There is no hidden algorithmic win left.

public static class Extens1
{
    /// <summary>
    /// Determines whether the text is a valid SQL object name.
    /// </summary>
    public static bool IsName(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (text.Length != text.Trim().Length) return false;

        int firstDot = text.IndexOf('.');
        if (firstDot < 0)
            return IsIdentifier(text.AsSpan());

        int lastDot = text.LastIndexOf('.');
        if (firstDot == lastDot)
        {
            var left = text.AsSpan(0, firstDot);
            var right = text.AsSpan(firstDot + 1);
            return IsIdentifier(left) && IsIdentifier(right);
        }

        int secondDot = text.IndexOf('.', firstDot + 1);
        if (secondDot != lastDot) return false;

        var part1 = text.AsSpan(0, firstDot);
        var part2 = text.AsSpan(firstDot + 1, secondDot - firstDot - 1);
        var part3 = text.AsSpan(secondDot + 1);

        return IsIdentifier(part1) &&
               IsIdentifier(part2) &&
               IsIdentifier(part3);
    }

    private static bool IsIdentifier(ReadOnlySpan<char> s)
    {
        if (s.Length == 0) return false;

        if (s[0] == '[')
        {
            if (s.Length < 2) return false;
            if (s[^1] != ']') return false;
            return true;
        }

        char c = s[0];
        if (!(c == '_' ||
              (c >= 'a' && c <= 'z') ||
              (c >= 'A' && c <= 'Z')))
            return false;

        for (int i = 1; i < s.Length; i++)
        {
            c = s[i];
            if (!(c == '_' ||
                  (c >= '0' && c <= '9') ||
                  (c >= 'a' && c <= 'z') ||
                  (c >= 'A' && c <= 'Z')))
                return false;
        }

        return true;
    }
}