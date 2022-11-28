using System;
using System.IO;
using System.Reflection;

namespace CoptTest.Tests
{
    internal static class Resource
    {
        static readonly string _resPrefix;
        static readonly string _trPrefix;

        static Resource()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = System.IO.Path.GetDirectoryName(codeBasePath);
            _resPrefix = System.IO.Path.Combine(dirPath, @"Resources\");

            _trPrefix = System.IO.Path.Combine(dirPath, @"Output\");
            Directory.CreateDirectory(_trPrefix);
        }

        public static string Path(string name) => _resPrefix + name;

        public static Stream Open(string name, FileMode mode = FileMode.Open) => File.Open(Path(name), mode);

        public static string ReadAllText(string name) => File.ReadAllText(Path(name));


        public static string TestResultPath(string name) => _trPrefix + name;

        public static Stream OpenTestResult(string name, FileMode mode = FileMode.Create) => File.Open(TestResultPath(name), mode);
    }
}
