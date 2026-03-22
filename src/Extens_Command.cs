// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace UkrGuru.Sql;

public static partial class Extens
{
    public static SqlCommand CreateCommand(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default)
    {
        SqlCommand command = new(tsql, connection);

        if (IsName(tsql)) command.CommandType = CommandType.StoredProcedure;
        if (data != null) command.Parameters.AddData(data);
        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;
    }

    public static bool IsName(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        // No leading/trailing whitespace
        if (text.Length != text.Trim().Length) return false;

        // Split by dot — but avoid allocations
        int firstDot = text.IndexOf('.');
        if (firstDot < 0)
        {
            // Just "name"
            return IsIdentifier(text.AsSpan());
        }

        int lastDot = text.LastIndexOf('.');

        if (firstDot == lastDot)
        {
            // "schema.name"
            var left = text.AsSpan(0, firstDot);
            var right = text.AsSpan(firstDot + 1);

            return IsIdentifier(left) && IsIdentifier(right);
        }

        // 3-part allowed: server.schema.name
        // But NOT 4-part
        int secondDot = text.IndexOf('.', firstDot + 1);
        if (secondDot != lastDot)
            return false; // more than 3 parts

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

        // Bracketed: [ ... ]
        if (s[0] == '[')
        {
            if (s.Length < 2) return false;
            if (s[^1] != ']') return false;
            return true; // Anything allowed inside
        }

        // Unquoted: first char must be letter or underscore
        char c = s[0];
        if (!(c == '_' ||
              (c >= 'a' && c <= 'z') ||
              (c >= 'A' && c <= 'Z')))
            return false;

        // Remaining chars: letters, digits, underscore
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