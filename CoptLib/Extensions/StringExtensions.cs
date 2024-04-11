using System;
using System.Collections.Generic;

namespace CoptLib.Extensions;

public static class StringExtensions
{
    public static bool StartsWithAny(this string str, IEnumerable<string> values, StringComparison comparisonType = default)
    {
        foreach (string val in values)
            if (str.StartsWith(val, comparisonType))
                return true;
        return false;
    }

    public static bool StartsWithAny(this string str, StringComparison comparisonType = default, params string[] values)
        => StartsWithAny(str, values, comparisonType);

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

    public static string[] SplitCamelCase(this string str)
    {
        var span = str.AsSpan();
        List<int> upperIndexes = [];
        
        for (int c = 0; c < span.Length; c++)
            if (char.IsUpper(span[c]))
                upperIndexes.Add(c);

        var segments = new string[upperIndexes.Count];
        for (int s = 0; s < segments.Length; s++)
        {
            int sNext = s + 1;
            var start = upperIndexes[s];
            var end = sNext < upperIndexes.Count
                ? upperIndexes[sNext] : str.Length;

            segments[s] = str[start..end];
        }

        return segments;
    }
}
