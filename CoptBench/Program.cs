using BenchmarkDotNet.Running;
using System;

namespace CoptBench;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        var interpreter = BenchmarkRunner.Run<Interpreter>();
    }
}