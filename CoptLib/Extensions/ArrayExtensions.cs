using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Extensions;

public static class ArrayExtensions
{
    public static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0) return;
        ArrayTraverse walker = new(array);
        do action(array, walker.Position);
        while (walker.Step());
    }

    public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> values)
    {
        var enumeratedValues = values.ToArray();
        return source.Any(s => enumeratedValues.Contains(s));
    }

    public static IList<T> Slice<T>(this IEnumerable<T> collection, int startIndex, int length)
    {
        if (collection is T[] array)
            return new ArraySegment<T>(array, startIndex, length);
            
        return collection.Skip(startIndex).Take(length).ToArray();
    }
}

internal class ArrayTraverse
{
    public int[] Position;
    private int[] _maxLengths;

    public ArrayTraverse(Array array)
    {
        _maxLengths = new int[array.Rank];
        for (int i = 0; i < array.Rank; ++i)
        {
            _maxLengths[i] = array.GetLength(i) - 1;
        }
        Position = new int[array.Rank];
    }

    public bool Step()
    {
        for (int i = 0; i < Position.Length; ++i)
        {
            if (Position[i] < _maxLengths[i])
            {
                Position[i]++;
                for (int j = 0; j < i; j++)
                {
                    Position[j] = 0;
                }
                return true;
            }
        }
        return false;
    }
}