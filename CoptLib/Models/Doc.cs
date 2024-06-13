using CommunityToolkit.Diagnostics;
using CoptLib.IO;
using CoptLib.Scripting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace CoptLib.Models;

[XmlRoot("Document")]
public class Doc : Definition, IContextualLoad
{
    private ILoadContext _context;
    private bool _transformed;

    public Doc(ILoadContext? context = null)
    {
        _context = context ?? new LoadContext();
        Translations = new(null)
        {
            DocContext = this
        };

        Name ??= Key ?? string.Empty;
        Parent = null;
        DocContext = this;
        IsExplicitlyDefined = true;
    }

    public string Name { get; set; }

    public Author? Author { get; set; }

    public TranslationCollectionSection Translations { get; set; }

    public IReadOnlyCollection<IDefinition> DirectDefinitions { get; set; } = [];
    
    public List<IScript<object>> Patterns { get; set; } = [];

    [NotNull]
    public ILoadContext? Context
    {
        get => _context;
        set
        {
            Guard.IsNotNull(value);
            _context = value;
        }
    }

    /// <summary>
    /// Gets the <see cref="IDefinition"/> associated with the given key.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    public IDefinition? LookupDefinition(string key) => Context.LookupDefinition(key, this);

    /// <summary>
    /// Adds an <see cref="IDefinition"/> to the current context, scoped to the
    /// this document if another definition with the same key exists.
    /// </summary>
    /// <param name="definition">The definition to add.</param>
    public void AddDefinition(IDefinition definition) => Context.AddDefinition(definition, this);

    /// <summary>
    /// Parses text commands and applies font conversions.
    /// </summary>
    /// <param name="force">
    /// When <see langword="true"/>, the transform will be reapplied
    /// even if the document has already been parsed. Note that this does
    /// not apply recursively, meaning that commands and font conversions
    /// that have already been applied and are unchanged will not be duplicated.
    /// </param>
    public void ApplyTransforms(bool force = false)
    {
        if (_transformed && !force)
            return;

        Transform(DirectDefinitions, Context);
        Transform(Translations.Children, Context);
        
        _transformed = true;
    }

    internal static void Transform(object part, ILoadContext? context)
    {
        if (part is ICommandOutput<object> partScript)
        {
            partScript.Execute(context);
            
            if (partScript.Output is not null)
                part = partScript.Output;
        }

        if (part is ISupportsTextCommands suppTextCmds)
            suppTextCmds.HandleCommands();

        if (part is System.Collections.IEnumerable manyParts)
            foreach (var childPart in manyParts)
                Transform(childPart, context);

        if (part is IMultilingual multilingual)
            multilingual.HandleFont();

        if (part is IDefinition {Key: not null, DocContext: not null} def)
            def.DocContext.AddDefinition(def);
    }
}