using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace CoptTest
{
    public class Writer
    {
        readonly string _resPrefix;

        public Writer()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            _resPrefix = Path.Combine(dirPath, @"Resources\");
        }

        [Theory]
        [InlineData("Let Us Praise the Lord.xml")]
        [InlineData("The First Hoos Lobsh.xml")]
        [InlineData("Watos Hymn for the Three Saintly Children.xml")]
        public void DocWriter_OpenAndWriteNoChanges(string file)
        {
            string xmlEx = File.ReadAllText(_resPrefix + file);
            Doc docEx = DocReader.ParseDocXml(XDocument.Parse(xmlEx));

            string xmlAc = DocWriter.WriteDocXml(docEx);
            Doc docAc = DocReader.ParseDocXml(XDocument.Parse(xmlAc));

            Assert.Equal(docEx.Name, docAc.Name);
            Assert.Equal(docEx.Uuid, docAc.Uuid);
            Assert.Equal(docEx.Parent, docAc.Parent);
            Assert.Equal(docEx.Definitions.Count, docAc.Definitions.Count);
            Assert.Equal(docEx.DirectDefinitions.Count, docAc.DirectDefinitions.Count);
            Assert.Equal(docEx.Translations.Children.Count, docAc.Translations.Children.Count);
        }
    }
}
