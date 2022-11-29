using BenchmarkDotNet.Attributes;
using CommandLine.Text;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptBench;

public class Interpreter
{
    public IEnumerable<object[]> IpaTranscribe_CopticUnicode_Samples { get; }

    public Interpreter()
    {
        IpaTranscribe_CopticUnicode_Samples =
            CoptTest.Interpreter.IpaTranscribe_CopticUnicode_Samples
            .Select(p => new object[] { p });
    }

    [Benchmark]
    [ArgumentsSource(nameof(IpaTranscribe_CopticUnicode_Samples))]
    public string IpaTranscribe_CopticUnicode_Single(string sample)
        => CopticInterpreter.IpaTranscribe(sample);

    [Benchmark]
    public void IpaTranscribe_CopticUnicode_Many()
    {
        Span<string> samples = CoptTest.Interpreter.IpaTranscribe_CopticUnicode_Samples;
        for (int i = 0; i < samples.Length; i++)
            CopticInterpreter.IpaTranscribe(samples[i]);
    }
}
