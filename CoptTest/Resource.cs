using System;
using System.IO;
using System.Reflection;
using OwlCore.Storage;
using OwlCore.Storage.SystemIO;

namespace CoptTest
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

        public static IFile Get(string name, FileMode mode = FileMode.Open)
        {
            string path = Path(name);
            File.Open(path, mode).Dispose();
            return new SystemFile(path);
        }
        

        public static string TestResultPath(string name) => _trPrefix + name;

        public static Stream OpenTestResult(string name, FileMode mode = FileMode.Create) => File.Open(TestResultPath(name), mode);

        public static IFile GetTestResult(string name, FileMode mode = FileMode.Create)
        {
            string path = TestResultPath(name);
            File.Open(path, mode).Dispose();
            return new SystemFile(path);
        }
    }
}
