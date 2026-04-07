//✅ 2.The V2 penalty is structural, not logical
//You tried everything reasonable:

//ASCII checks ✅
//no regex ✅
//no spans ✅
//fewer dots ✅
//tightly written loops ✅

//Yet V2 still loses ~22 ns.
public static class Extens2
{
    public static bool IsName(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        // No leading or trailing whitespace
        if (char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[^1]))
            return false;

        int dotCount = 0;
        int partStart = 0;
        int len = text.Length;

        for (int i = 0; i <= len; i++)
        {
            if (i == len || text[i] == '.')
            {
                int partLen = i - partStart;
                if (partLen == 0)
                    return false;

                if (!IsIdentifier(text, partStart, partLen))
                    return false;

                if (i < len && ++dotCount == 3)
                    return false; // more than 3 parts

                partStart = i + 1;
            }
        }

        return true;
    }

    private static bool IsIdentifier(string text, int start, int length)
    {
        char first = text[start];
        char last = text[start + length - 1];

        // No whitespace around parts
        if (first <= ' ' || last <= ' ')
            return false;

        // Bracketed identifier: [anything]
        if (first == '[')
            return length >= 2 && last == ']';

        // Unquoted identifier: [_A-Za-z][_A-Za-z0-9]*
        if (!(first == '_' ||
              (first >= 'A' && first <= 'Z') ||
              (first >= 'a' && first <= 'z')))
            return false;

        for (int i = start + 1, end = start + length; i < end; i++)
        {
            char c = text[i];
            if (!(c == '_' ||
                  (c >= '0' && c <= '9') ||
                  (c >= 'A' && c <= 'Z') ||
                  (c >= 'a' && c <= 'z')))
                return false;
        }

        return true;
    }
}
