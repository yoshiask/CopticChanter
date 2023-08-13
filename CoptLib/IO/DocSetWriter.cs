using CoptLib.Models;
using OwlCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CoptLib.IO;

public class DocSetWriter
{
    internal const string DocsDirectory = "docs";
    internal const string IndexEntry = "index";
    internal const string MetaEntry = "meta.xml";

    public DocSet Set { get; }

    public DocSetWriter(DocSet set)
    {
        Set = set;
    }

    public DocSetWriter(string key, string name, IEnumerable<Doc>? docs = null)
    {
        Set = new(key, name, docs);
    }

    public async Task WriteAsync(IModifiableFolder rootFolder)
    {
        // Begin building index
        StringBuilder sb = new();

        var docs = await rootFolder.CreateFolderAsync(DocsDirectory);
        if (docs is not IModifiableFolder docsDir)
            throw new InvalidOperationException($"'{rootFolder.Id}' and all its subfolders must be modifiable.");

        foreach (var doc in Set.IncludedDocs)
        {
            // Write each Document to its own entry
            var docFile = await docsDir.CreateFileAsync(doc.Key!);
            using var docFileStream = await docFile.OpenStreamAsync(FileAccess.Write);
            DocWriter.WriteDocXml(doc, docFileStream);

            // Write the Document location and name to index
            string relativePath = await docsDir.GetRelativePathToAsync(docFile);
            sb.AppendLine(relativePath);
            sb.AppendLine(doc.Name);
        }

        // Write the index to an entry
        var indexFile = await rootFolder.CreateFileAsync(IndexEntry);
        using (var indexFileStream = await indexFile.OpenStreamAsync(FileAccess.Write))
        using (StreamWriter sw = new(indexFileStream))
        {
            await sw.WriteAsync(sb.ToString());
        }

        // Write additional metadata
        var metaFile = await rootFolder.CreateFileAsync(MetaEntry);
        using (var metaFileStream = await metaFile.OpenStreamAsync(FileAccess.Write))
        using (XmlTextWriter xw = new(metaFileStream, Encoding.Unicode))
        {
            var xml = Set.Serialize();
            xml.WriteTo(xw);
        }
    }
}