using BenchmarkDotNet.Attributes;
using CoptLib.Writing.Linguistics.Analyzers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoptBench;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Interpreter
{
    public IEnumerable<object[]> IpaTranscribe_CopticUnicode_Samples { get; }

    public CopticGrecoBohairicAnalyzer Analyzer { get; }

    public Interpreter()
    {
        IpaTranscribe_CopticUnicode_Samples =
            CoptTest.Interpreter.IpaTranscribe_CopticUnicode_Samples
            .Select(p => new object[] { p });

        Analyzer = new();
    }

    [Benchmark]
    [ArgumentsSource(nameof(IpaTranscribe_CopticUnicode_Samples))]
    public string IpaTranscribe_CopticUnicode_SingleCached(string sample)
        => Analyzer.IpaTranscribe(sample);

    [Benchmark]
    public void IpaTranscribe_CopticUnicode_ManyCached()
    {
        foreach (var sample in CoptTest.Interpreter.IpaTranscribe_CopticUnicode_Samples)
            Analyzer.IpaTranscribe(sample[0].ToString()!);
    }
}
