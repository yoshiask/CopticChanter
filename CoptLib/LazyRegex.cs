using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace CoptLib;

public class LazyRegex([StringSyntax(StringSyntaxAttribute.Regex)] string regex)
{
    private static readonly ConcurrentDictionary<string, Regex> _cache = new();

    private LazyRegex(Regex rx) : this(rx.ToString()) =>
        _cache.AddOrUpdate(RegexString,
            s => rx,
            (_, existing) => rx);

    public string RegexString { get; } = regex;

    public Regex Get()
    {
        if (!_cache.TryGetValue(RegexString, out var rx))
            return rx;

        return _cache.AddOrUpdate(RegexString,
            s => new(s, RegexOptions.Compiled),
            (_, existing) => existing);
    }

    public static implicit operator Regex(LazyRegex lrx) => lrx.Get();
    public static implicit operator LazyRegex(Regex rx) => new(rx);
}
