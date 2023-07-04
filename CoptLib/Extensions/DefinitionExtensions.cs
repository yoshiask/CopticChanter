using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using OwlCore.Extensions;
using System;

namespace CoptLib.Extensions
{
    public static class DefinitionExtensions
    {
        public static IDefinition Select(this IDefinition rootDef, Action<IDefinition> func)
        {
            var newDef = rootDef.Copy(false);

            if (newDef is IContentCollectionContainer defs)
                for (int i = 0; i < defs.Children.Count; i++)
                    defs.Children[i] = defs.Children[i].Select(func) as ContentPart;

            if (newDef is Section {Title: not null} section)
                section.SetTitle(section.Title.Select(func) as IContent);

            if (newDef is IContent content)
                for (int i = 0; i < content.Inlines.Count; i++)
                    content.Inlines[i] = content.Inlines[i].Select(func) as Inline;

            if (newDef is Span span)
                for (int i = 0; i < span.Inlines.Count; i++)
                    span.Inlines[i] = span.Inlines[i].Select(func) as Inline;

            func(newDef);

            return newDef;
        }

        public static LanguageInfo GetLanguage(this IDefinition def)
        {
            if (def is IMultilingual multi && LangIsNotNullOrDefault(def))
                return multi.Language;

            var ancestor = def.CrawlBy(d => d.Parent, LangIsNotNullOrDefault);

            return (ancestor as IMultilingual)?.Language ?? LanguageInfo.Default;
        }

        /// <summary>
        /// Registers the given definition as a reference of this definition.
        /// </summary>
        /// <param name="def">The referenced definition.</param>
        /// <param name="referencee">The definition referencing it.</param>
        public static void RegisterReference(this IDefinition def, IDefinition referencee)
            => def.References.Add(referencee);

        private static bool LangIsNotNullOrDefault(IDefinition def)
            => def is IMultilingual multi
            && multi?.Language is not null 
            && multi.Language.Known != KnownLanguage.Default;
    }
}
