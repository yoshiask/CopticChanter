using System;
using System.Collections.Generic;

namespace CoptLib.Extensions
{
    public static class ArrayExtensions
    {
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        /// <summary>
        /// Flattens the given enumerable using a pre-order depth-first traversal.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="topNode">The item to begin flattening from.</param>
        /// <param name="elementSelector">A function that returns the children of the current node.</param>
        public static IEnumerable<T> Flatten<T>(this T topNode, Func<T, IEnumerable<T>> elementSelector)
        {
            // https://stackoverflow.com/a/31881243
            Stack<IEnumerator<T>> stack = new();
            var e = elementSelector(topNode).GetEnumerator();
            yield return topNode;

            try
            {

                while (true)
                {
                    while (e.MoveNext())
                    {
                        var item = e.Current;
                        yield return item;

                        var elements = elementSelector(item);
                        if (elements == null)
                            continue;

                        stack.Push(e);
                        e = elements.GetEnumerator();
                    }

                    if (stack.Count == 0)
                        break;

                    e.Dispose();
                    e = stack.Pop();
                }
            }
            finally
            {
                e.Dispose();

                while (stack.Count != 0)
                    stack.Pop().Dispose();
            }
        }

        public static IEnumerable<(T elem, int index)> WithIndex<T>(this IEnumerable<T> source, int start = 0)
        {
            int i = start;
            foreach (var elem in source)
                yield return (elem, i++);
        }

        public static IEnumerable<TTarget> ElementsAs<TSource, TTarget>(this IEnumerable<TSource> source)
        {
            foreach (var elem in source)
                if (elem is TTarget elemTarget)
                    yield return elemTarget;
        }
    }

    internal class ArrayTraverse
    {
        public int[] Position;
        private int[] maxLengths;

        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                maxLengths[i] = array.GetLength(i) - 1;
            }
            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < Position.Length; ++i)
            {
                if (Position[i] < maxLengths[i])
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
}
