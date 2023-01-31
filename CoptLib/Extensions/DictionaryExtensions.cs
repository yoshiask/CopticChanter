using System.Collections.Generic;

namespace CoptLib.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<T2, T1> SwitchColumns<T1, T2>(this Dictionary<T1, T2> dictionary)
    {
        Dictionary<T2, T1> output = new();

        foreach (KeyValuePair<T1, T2> pair in dictionary)
            output.Add(pair.Value, pair.Key);

        return output;
    }
}
