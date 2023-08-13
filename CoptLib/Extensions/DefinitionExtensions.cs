using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using OwlCore.Extensions;
using System;

namespace CoptLib.Extensions;

public static class DefinitionExtensions
{
    public static IDefinition Select(this IDefinition rootDef, Action<IDefinition> func)
    {
        var newDef = rootDef.Copy()!;

        if (newDef is IContentCollectionContainer defs)
            for (int i = 0; i < defs.Children.Count; i++)
                defs.Children[i] = (ContentPart) defs.Children[i].Select(func);

        if (newDef is Section {Title: not null} section)
            section.SetTitle(section.Title.Select(func) as IContent);

        if (newDef is IContent content)
            for (int i = 0; i < content.Inlines.Count; i++)
                content.Inlines[i] = (Inline)content.Inlines[i].Select(func);

        if (newDef is Span span)
            for (int i = 0; i < span.Inlines.Count; i++)
                span.Inlines[i] = (Inline)span.Inlines[i].Select(func);

        func(newDef);

        return newDef;
    }

    public static LanguageInfo GetLanguage(this IDefinition def)
    {
        if (def is IMultilingual multi && !LanguageInfo.IsNullOrDefault(multi.Language))
            return multi.Language;

        var ancestor = def.CrawlBy(d => d.Parent,
            d => d is IMultilingual m && !LanguageInfo.IsNullOrDefault(m.Language));

        return (ancestor as IMultilingual)?.Language ?? LanguageInfo.Default;
    }
        
    public static string? GetFont(this IDefinition def)
    {
        if (def is IMultilingual {Font: not null} multi)
            return multi.Font;

        var ancestor = def.CrawlBy(d => d.Parent,
            d => d is IMultilingual {Font: not null});

        return (ancestor as IMultilingual)?.Font;
    }

    /// <summary>
    /// Registers the given definition as a reference of this definition.
    /// </summary>
    /// <param name="def">The referenced definition.</param>
    /// <param name="referencee">The definition referencing it.</param>
    public static void RegisterReference(this IDefinition def, IDefinition referencee)
        => def.References.Add(referencee);
}