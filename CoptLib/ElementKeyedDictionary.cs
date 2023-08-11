using System;
using System.Collections.Generic;

namespace CoptLib;

/// <summary>
/// A <see cref="SortedDictionary{TKey,TValue}"/>
/// </summary>
/// <typeparam name="TKey">
/// The type of the keys in the dictionary.
/// </typeparam>
/// <typeparam name="TValue">
/// The type of the values in the dictionary.
/// </typeparam>
public class ElementKeyedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public ElementKeyedDictionary(Func<TValue, TKey> keySelector)
    {
        KeySelector = keySelector;
    }

    /// <summary>
    /// Gets the key given the entire value.
    /// </summary>
    public Func<TValue, TKey> KeySelector { get; }

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="value">
    /// The value of the element to add.
    /// </param>
    public void Add(TValue value) => Add(KeySelector(value), value);
}