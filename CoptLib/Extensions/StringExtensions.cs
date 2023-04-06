using System;
using System.Collections.Generic;

namespace CoptLib.Extensions;

public static class StringExtensions
{
    public static bool StartsWithAny(this string str, IEnumerable<string> values)
    {
        foreach (string val in values)
            if (str.StartsWith(val))
                return true;
        return false;
    }

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
        // Coutesy of https://stackoverflow.com/a/3143036
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
}
