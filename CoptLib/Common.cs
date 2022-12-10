using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CoptLib
{
    public static class Common
    {

    }

    public static class XmlTools
    {
        public static string ToXmlString<T>(this T input)
        {
            using var writer = new StringWriter();
            input.ToXml(writer);
            return writer.ToString();
        }

        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }
    }

    public static class DateTimeExtensions
    {
        ///<summary>Gets the first week day following a date.</summary>
        ///<param name="date">The date.</param>
        ///<param name="dayOfWeek">The day of week to return.</param>
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }
    }

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
                    start = val;
                    return str.Remove(0, val.Length);
                }
            }

            start = null;
            return str;
        }

        public static void AddRange<T>(this ICollection<T> dst, IEnumerable<T> src)
        {
            foreach (T t in src)
                dst.Add(t);
        }
    }
}
