using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CoptLib;

public class NothingDictionary<T> : IReadOnlyDictionary<T, T>
{
    public IEnumerator<KeyValuePair<T, T>> GetEnumerator() => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => int.MaxValue;

    public bool ContainsKey(T key) => true;

    public bool TryGetValue(T key, [UnscopedRef] out T value)
    {
        value = key;
        return true;
    }

    public T this[T key] => key;

    public IEnumerable<T> Keys => throw new NotSupportedException();

    public IEnumerable<T> Values => throw new NotSupportedException();
}