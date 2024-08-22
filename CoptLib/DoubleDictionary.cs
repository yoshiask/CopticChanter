using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib;

public class DoubleDictionary<TLeft, TRight> : ICollection<(TLeft, TRight)>, IDictionary<TLeft, TRight>
{
    private readonly Dictionary<TLeft, TRight> _leftDict;
    private readonly Dictionary<TRight, TLeft> _rightDict;

    public int Count => _leftDict.Count;
    public bool IsReadOnly => false;

    public ICollection<TLeft> Keys => ((IDictionary<TLeft, TRight>)_leftDict).Keys;

    public ICollection<TRight> Values => ((IDictionary<TLeft, TRight>)_leftDict).Values;

    TRight IDictionary<TLeft, TRight>.this[TLeft key]
    {
        get => ((IDictionary<TLeft, TRight>)_leftDict)[key];
        set => ((IDictionary<TLeft, TRight>)_leftDict)[key] = value;
    }

    public IReadOnlyDictionary<TLeft, TRight> GetLeftDictionary() => _leftDict;
    public IReadOnlyDictionary<TRight, TLeft> GetRightDictionary() => _rightDict;

    public DoubleDictionary(int capacity = 0)
    {
        _leftDict = new(capacity);
        _rightDict = new(capacity);
    }

    public void Add(TLeft l, TRight r)
    {
        _leftDict[l] = r;
        _rightDict[r] = l;
    }

    public TRight this[TLeft l]
    {
        get => _leftDict[l];
        set => Add(l, value);
    }

    public TLeft this[TRight r]
    {
        get => _rightDict[r];
        set => Add(value, r);
    }

    public bool Contains(TLeft l) => _leftDict.ContainsKey(l);
    public bool Contains(TRight r) => _rightDict.ContainsKey(r);

    public bool TryGetRight(TLeft l, out TRight r) => _leftDict.TryGetValue(l, out r);
    public bool TryGetLeft(TRight r, out TLeft l) => _rightDict.TryGetValue(r, out l);

    public bool Remove(TLeft l)
    {
        if (!TryGetRight(l, out var r))
            return false;
        _leftDict.Remove(l);

        return _rightDict.Remove(r);
    }

    public bool Remove(TRight r)
    {
        if (!TryGetLeft(r, out var l))
            return false;
        _rightDict.Remove(r);

        return _leftDict.Remove(l);
    }

    public IEnumerator<(TLeft, TRight)> GetEnumerator() => _leftDict
        .Select(kv => (kv.Key, kv.Value))
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add((TLeft, TRight) item) => Add(item.Item1, item.Item2);

    public void Clear()
    {
        _leftDict.Clear();
        _rightDict.Clear();
    }

    public bool Contains((TLeft, TRight) item)
        => _leftDict.TryGetValue(item.Item1, out var r) && r!.Equals(item.Item2);

    public void CopyTo((TLeft, TRight)[] array, int arrayIndex)
    {
        var i = arrayIndex;
        foreach (var pair in this)
            array[i++] = pair;
    }

    public bool Remove((TLeft, TRight) item) => Remove(item.Item1) && Remove(item.Item2);

    public bool ContainsKey(TLeft key)
    {
        return ((IDictionary<TLeft, TRight>)_leftDict).ContainsKey(key);
    }

    public bool TryGetValue(TLeft key, out TRight value)
    {
        return ((IDictionary<TLeft, TRight>)_leftDict).TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<TLeft, TRight> item)
    {
        ((ICollection<KeyValuePair<TLeft, TRight>>)_leftDict).Add(item);
    }

    public bool Contains(KeyValuePair<TLeft, TRight> item)
    {
        return ((ICollection<KeyValuePair<TLeft, TRight>>)_leftDict).Contains(item);
    }

    public void CopyTo(KeyValuePair<TLeft, TRight>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TLeft, TRight>>)_leftDict).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TLeft, TRight> item)
    {
        return ((ICollection<KeyValuePair<TLeft, TRight>>)_leftDict).Remove(item);
    }

    IEnumerator<KeyValuePair<TLeft, TRight>> IEnumerable<KeyValuePair<TLeft, TRight>>.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TLeft, TRight>>)_leftDict).GetEnumerator();
    }
}