using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib
{
    public static class DictionaryTools
    {
        public static Dictionary<T2, T1> SwitchColumns<T1, T2>(Dictionary<T1, T2> dictionary)
        {
            Dictionary<T2, T1> output = new Dictionary<T2, T1>();
            foreach (KeyValuePair<T1, T2> pair in dictionary)
            {
                output.Add(pair.Value, pair.Key);
            }
            return output;
        }
    }

    public static class ArrayExtensions
    {
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> values)
        {
            foreach (T s in source)
                if (values.Contains(s))
                    return true;
            return false;
        }

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
                    start = str.Substring(0, val.Length);
                    return str.Substring(val.Length);
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
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
                yield return s.Substring(start);
        }

        public static void AddRange<T>(this ICollection<T> dst, IEnumerable<T> src)
        {
            foreach (T t in src)
                dst.Add(t);
        }
    }
}
