using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CoptLib.Extensions;

public static class StringExtensions
{
    public static bool StartsWithAny(this string str, IEnumerable<string> values, [NotNullWhen(true)] out string? prefix, StringComparison comparisonType = default)
    {
        foreach (string val in values)
        {
            if (!str.StartsWith(val, comparisonType))
                continue;

            prefix = val;
            return true;
        }

        prefix = null;
        return false;
    }

    public static bool StartsWithAny(this string str, StringComparison comparisonType = default, params string[] values)
        => StartsWithAny(str, values, out _, comparisonType);

    public static bool EndsWithAny(this string str, IEnumerable<string> values)
    {
        foreach (string val in values)
            if (str.EndsWith(val))
                return true;
        return false;
    }

    public static string StripAnyFromStart(this string str, IEnumerable<string> values, out string? start, StringComparison comparisonType = default)
    {
        foreach (string val in values)
        {
            if (str.StartsWith(val, comparisonType))
            {
                start = str[..val.Length];
                return str[val.Length..];
            }
        }

        start = null;
        return str;
    }

    public static IEnumerable<string> SplitAndKeep(this string s, char[] separator)
    {
        // Courtesy of https://stackoverflow.com/a/3143036
        int start = 0, index;

        while ((index = s.IndexOfAny(separator, start)) >= 0)
        {
            if (index - start > 0)
                yield return s[start..index];
            yield return s.Substring(index, 1);
            start = index + 1;
        }

        if (start < s.Length)
            yield return s[start..];
    }

    public static (string Before, string? After) SplitAtChar(this string s, char separator)
    {
        var index = s.IndexOf(separator);
        return index < 0
            ? (s, null)
            : (s[..index], s[(index + 1)..]);
    }

    public static IEnumerable<string> SplitAlongCapitals(this string s)
    {
        int lastCapital = 0;
        for (int i = 0; i < s.Length;)
        {
            char ch = s[i];
            if (!char.IsUpper(ch))
                continue;

            yield return s[lastCapital..++i];
            lastCapital = i;
        }
    }

    public static string Substring(this string s, Range range)
    {
        (var offset, var length)= range.GetOffsetAndLength(s.Length);
        return s.Substring(offset, length);
    }
}
