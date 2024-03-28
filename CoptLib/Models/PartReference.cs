using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Models;

public class PartReference(IDefinition? parent) : ContentPart(parent), ICommandOutput<IDefinition>
{
    private IDefinition? _referencedDef;
    private ContentPart? _referencedPart;

    /// <summary>
    /// The key of a definitions used to populate the collection. May contain nested text commands.
    /// </summary>
    public SimpleContent? Source { get; set; }

    public ContentPart? ReferencedPart => Execute(null) as ContentPart;

    public IDefinition? Output => GetPreferredDefinition();

    public bool Evaluated => CommandsHandled;

    public override void HandleCommands()
    {
        if (Source != null && !CommandsHandled)
        {
            Source.HandleCommands();

            if (DocContext is null)
                throw new InvalidOperationException("A DocContext is required for looking up definitions.");

            var defKey = Source.GetText();
            var def = DocContext.Context.LookupDefinition(defKey, DocContext);

            _referencedDef = def;
            def?.RegisterReference(this);
        }
    }

    public override void HandleFont() => ReferencedPart?.HandleFont();

    public override string ToString() => GetPreferredDefinition()?.ToString() ?? "";

    public IDefinition? Execute(ILoadContext? context)
    {
        HandleCommands();
        return _referencedPart ??= GetPart();
    }

    private IDefinition? GetPreferredDefinition() => ReferencedPart ?? _referencedDef ?? Source;

    private ContentPart? GetPart()
    {
        var def = _referencedDef;

        if (def is Text.TranslationRunCollection outTranslations)
            def = outTranslations.GetByLanguage(Language);

        switch (def)
        {
            case Text.Run outRun:
                Stanza stanza1 = new(this)
                {
                    Inlines = [outRun],
                    CommandsHandled = true
                };
                return stanza1;

            case Text.Span outSpan:
                Stanza stanza2 = new(this)
                {
                    Inlines = outSpan.Inlines,
                    CommandsHandled = outSpan.Inlines
                        .All(i => i is not Text.InlineCommand c || c.Command is null || c.Command.Evaluated)
                };
                return stanza2;
        }

        return _referencedDef as ContentPart;
    }
}
