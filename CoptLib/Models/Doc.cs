using CommunityToolkit.Diagnostics;
using CoptLib.IO;
using CoptLib.Scripting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using CoptLib.Extensions;

namespace CoptLib.Models;

[XmlRoot("Document")]
public class Doc : Definition, IContextualLoad
{
    private LoadContextBase _context;
    private bool _transformed;

    public Doc(LoadContextBase? context = null)
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

    public IReadOnlyCollection<IDefinition> DirectDefinitions { get; set; } = System.Array.Empty<IDefinition>();

    [NotNull]
    public LoadContextBase? Context
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

    internal static void Transform(object part, LoadContextBase? context)
    {
        if (part is ICommandOutput<object> partScript)
        {
            partScript.Execute(context);
            
            if (partScript.Output is not null)
                part = partScript.Output;
        }

        if (part is IContentCollectionContainer partCollection)
        {
            var collSrc = partCollection.Source;
            if (collSrc is {CommandsHandled: false})
            {
                // Populate the collection with items from the source.
                // This is done before commands are parsed, just in
                // case the generated content contains commands.
                collSrc.HandleCommands();
                var cmd = collSrc.Commands.LastOrDefault();

                if (cmd?.Output != null)
                {
                    bool hasExplicitChildren = partCollection.Children.Count > 0;

                    switch (cmd.Output)
                    {
                        case IContentCollectionContainer cmdOutCollection:
                            partCollection.Children.AddRange(cmdOutCollection.Children);
                            cmd.Output.RegisterReference(partCollection);
                            break;
                        case ContentPart cmdOutPart:
                            partCollection.Children.Add(cmdOutPart);
                            cmd.Output.RegisterReference(partCollection);
                            break;
                        case Text.Run cmdOutRun:
                            Stanza stanza1 = new(partCollection)
                            {
                                Inlines = new() { cmdOutRun },
                                CommandsHandled = true
                            };
                            cmd.Output.RegisterReference(stanza1);
                            partCollection.Children.Add(stanza1);
                            break;
                        case Text.Span cmdOutSpan:
                            Stanza stanza2 = new(partCollection)
                            {
                                Inlines = cmdOutSpan.Inlines,
                                CommandsHandled = cmdOutSpan.Inlines
                                    .All(i => i is not Text.InlineCommand c || c.Command is null || c.Command.Evaluated)
                            };
                            cmd.Output.RegisterReference(stanza2);
                            partCollection.Children.Add(stanza2);
                            break;
                    }

                    // If the collection doesn't have any explicit elements (in other words,
                    // it's only children came from the source), inherit the command's properties
                    if (!hasExplicitChildren)
                    {
                        if (cmd.Output is IMultilingual cmdMulti && part is IMultilingual partMulti)
                        {
                            partMulti.Language = cmdMulti.Language;
                            partMulti.Font = cmdMulti.Font;
                        }

                        if (cmd.Output is Section cmdSection && part is Section partSection)
                            partSection.SetTitle(cmdSection.Title);

                        if (cmd.Output is ContentPart cmdContentPart && part is ContentPart partContentPart)
                            partContentPart.RoleName = cmdContentPart.RoleName;
                    }
                }
            }
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