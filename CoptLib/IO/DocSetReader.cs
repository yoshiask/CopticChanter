using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Linq;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.IO
{
    public class DocSetReader
    {
        private string ZipPath
        {
            get;
        }
        private Stream ZipStream
        {
            get;
        }
        public string Name
        {
            get;
        }

        public DocSetReader(string name, string zipPath)
        {
            Name = name;
            ZipPath = zipPath;
        }
        public DocSetReader(string name, Stream zipStream)
        {
            Name = name;
            ZipStream = zipStream;
        }

        /// <param name="tempPath">Path.Combine(
        /// Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        ///         "Coptic Chanter", "Doc Creator", "temp")</param>
        /// <returns></returns>
        public ReadResults Read(string tempPath)
        {
            string folderPath = Path.Combine(tempPath, Name);
            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.EnumerateFiles(folderPath))
                {
                    File.Delete(file);
                }
                Directory.Delete(folderPath);
            }
            Directory.CreateDirectory(folderPath);

            bool isSuccess = ZipStream == null ? UnzipFile(ZipPath, folderPath) : UnzipFile(ZipStream, folderPath);
            if (isSuccess)
            {
                var results = new ReadResults();
                List<string> files = Directory.EnumerateFiles(folderPath).ToList();
                if (files.Contains(Path.Combine(folderPath, "index.xml")))
                {
                    // Create an instance of the XmlSerializer class;
                    // specify the type of object to be deserialized.
                    XmlSerializer serializer = new XmlSerializer(typeof(Index));

                    // A FileStream is needed to read the XML document.
                    string text = File.ReadAllText(Path.Combine(folderPath, "index.xml"));

                    //Use the Deserialize method to restore the object's state with
                    //data from the XML document.
                    results.Index = (Index)serializer.Deserialize(XDocument.Parse(text).CreateReader());

                    foreach (string filename in files)
                    {
                        if (filename != Path.Combine(folderPath, "index.xml") && !filename.EndsWith(".zip"))
                        {
                            try
                            {
                                var doc = DocReader.ReadDocXml(filename);
                                results.IncludedDocs.Add(doc);

                                /*// Create an instance of the XmlSerializer class;
                                // specify the type of object to be deserialized.
                                XmlSerializer serializerDoc = new XmlSerializer(typeof(DocXML));

                                // A FileStream is needed to read the XML document.
                                string textDoc = File.ReadAllText(filename);

                                //Use the Deserialize method to restore the object's state with
                                //data from the XML document.
                                var readerDoc = XDocument.Parse(textDoc).CreateReader();
                                var serialDoc = (DocXML)serializerDoc.Deserialize(readerDoc);
                                results.IncludedDocs.Add(serialDoc);*/
                            }
                            catch (Exception ex)
                            {
                                Output.WriteLine("Error: ", ex.Message);
                                Output.WriteLine("Unexpected file in set");
                            }
                        }
                    }

                    return results;
                }
                else
                {
                    Output.WriteLine("Set file not valid: No index found");
                }
            }
            return null;
        }

        /// <summary>
        /// Extracts a zip file in the specified directory
        /// </summary>
        /// <param name="zipPath">Zip file to extract</param>
        /// <param name="extractPath">Path to extract zip file to</param>
        private bool UnzipFile(string zipPath, string extractPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts a zip file in the specified directory
        /// </summary>
        /// <param name="zipStream">Zip stream to extract</param>
        /// <param name="extractPath">Path to extract zip file to</param>
        private bool UnzipFile(Stream zipStream, string extractPath)
        {
            try
            {
                new ZipArchive(zipStream).ExtractToDirectory(extractPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public class ReadResults
        {
            public Index Index;
            public List<Doc> IncludedDocs = new List<Doc>();
        }
    }
}
