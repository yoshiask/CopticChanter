using System;
using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using NodaTime;
using OwlCore.Storage;

namespace CoptLib.IO;

public abstract class LoadContextBase : ILoadContext
{
    private readonly Dictionary<string, IDefinition> _definitions = new();
    private readonly List<Doc> _loadedDocs = new();
    private LocalDate? _nowOverride;

    public IReadOnlyDictionary<string, IDefinition> Definitions => _definitions;

    public IReadOnlyList<Doc> LoadedDocs => _loadedDocs;

    public LocalDate CurrentDate => _nowOverride ?? DateHelper.TodayCoptic();

    public void SetDate(LocalDateTime? nowOverride) => _nowOverride = nowOverride?.ToCopticDate();

    private static string BuildScopedKey(string key, IContextualLoad ctxItem)
    {
        Guard.IsNotNull(key, nameof(key));
        Guard.IsNotNull(ctxItem, nameof(ctxItem));

        return $"{key};Scope='{ctxItem.Key}'";
    }

    public void AddDefinition(IDefinition definition, IContextualLoad? contextualItem)
    {
        var key = definition.Key
                  ?? throw new ArgumentNullException(nameof(definition), "Definition must specify a key.");

        // If key already exists and a ctxItem was specified, scope the def
        if (contextualItem is not null && _definitions.ContainsKey(key))
            key = BuildScopedKey(key, contextualItem);

        _definitions[key] = definition;
    }

    public void Clear()
    {
        _definitions.Clear();
        _loadedDocs.Clear();
    }

    public void AddDoc(Doc doc)
    {
        if (_loadedDocs.Contains(doc))
            return;

        _loadedDocs.Add(doc);

        AddDefinition(doc, doc);
        foreach (var def in doc.DirectDefinitions)
            AddDefinition(def, doc);
    }

    public Doc LoadDoc(Stream file)
    {
        var doc = DocReader.ReadDocXml(file, this);
        AddDoc(doc);
        return doc;
    }

    public async Task<Doc> LoadDoc(IFile file)
    {
        using var stream = await file.OpenStreamAsync();
        return LoadDoc(stream);
    }

    public Doc LoadDoc(string path)
    {
        var doc = DocReader.ReadDocXml(path, this);
        AddDoc(doc);
        return doc;
    }

    public Doc LoadDocFromXml(string xml)
    {
        var doc = DocReader.ParseDocXml(xml, this);
        AddDoc(doc);
        return doc;
    }

    public Doc LoadDocFromXml(System.Xml.Linq.XDocument xml)
    {
        var doc = DocReader.ParseDocXml(xml, this);
        AddDoc(doc);
        return doc;
    }

    public virtual IDefinition? LookupDefinition(string key, IContextualLoad? contextualItem = null)
    {
        Guard.IsNotNullOrEmpty(key, nameof(key));

        // Check for scoped entry
        if (contextualItem is not null && _definitions.TryGetValue(BuildScopedKey(key, contextualItem), out var def))
            return def;

        return _definitions.TryGetValue(key, out def) ? def : null;
    }

    public bool TryLookupDefinition(string key, [NotNullWhen(true)] out IDefinition? def, IContextualLoad? contextualItem = null)
    {
        def = LookupDefinition(key);
        return def is not null;
    }
}