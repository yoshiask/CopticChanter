using CoptLib.Models;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.IO
{
    public class DocSetWriter
    {
        /// <summary>
        /// An Index XML object that contains data to write to set
        /// </summary>
        public Index Index
        {
            get;
            private set;
        }

        public DocSetWriter(Index index = null)
        {
            Index = index;
        }

        /// <summary>
        /// Writes the data written to Index and Docs variable
        /// </summary>
        /// <returns></returns>
        public bool Write(string path)
        {
            if (Index != null)
            {
                string rootPath = path.Replace(".zip", "");
                Directory.CreateDirectory(rootPath);

                // Let's sort out the documents first
                foreach (IndexDoc indexDoc in Index.IncludedDocs)
                {
                    XmlSerializer docSerializer = new XmlSerializer(typeof(Doc));
                    TextWriter docWriter = new StreamWriter(new FileStream(Path.Combine(rootPath, indexDoc.Name + ".xml"), FileMode.Create));
                    docSerializer.Serialize(docWriter, DocReader.AllDocs[indexDoc.Uuid]);
                    docWriter.Dispose();
                }

                // Now save the index
                XmlSerializer serializer = new XmlSerializer(typeof(Index));
                TextWriter writer = new StreamWriter(new FileStream(Path.Combine(rootPath, "index.xml"), FileMode.Create));
                serializer.Serialize(writer, Index);
                writer.Dispose();

                // Files are saved, now compess to zip
                ZipFile(rootPath, Path.Combine(Path.GetDirectoryName(rootPath), Index.Name + ".zip"));

                // Delete temp. directory
                foreach (string filepath in Directory.GetFiles(rootPath))
                {
                    File.Delete(filepath);
                }
                Directory.Delete(rootPath);

                return true;
            }
            else
            {
                Output.WriteLine("Index or Content is null");
                return false;
            }
        }

        public void AddContent(Doc xml)
        {
            Index.IncludedDocs.Add(new IndexDoc()
            {
                Name = xml.Name,
                Uuid = xml.Uuid,
            });
        }

        public void AddContent(IEnumerable<Doc> docs)
        {
            foreach (Doc xml in docs)
            {
                Index.IncludedDocs.Add(new IndexDoc()
                {
                    Name = xml.Name,
                    Uuid = xml.Uuid
                });
            }
        }

        public void ClearContent()
        {
            Index.IncludedDocs.Clear();
        }

        public void SetIndex(Index index)
        {
            Index = index;
        }

        public void ClearIndex()
        {
            Index = null;
        }

        /// <summary>
        /// Creates a zip file in the specified directory
        /// </summary>
        /// <param name="path">Folder to compress</param>
        /// <param name="zipPath">File to write to</param>
        private void ZipFile(string path, string zipPath)
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
            System.IO.Compression.ZipFile.CreateFromDirectory(path, zipPath);
        }
    }
}
