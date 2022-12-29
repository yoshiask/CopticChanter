using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using System;

namespace CoptLib.Extensions
{
    public static class DefinitionExtensions
    {
        public static IDefinition Select(this IDefinition rootDef, Action<IDefinition> func)
        {
            var newDef = rootDef.Copy(false);
            if (newDef is IContentCollectionContainer defs)
            {
                for (int i = 0; i < defs.Children.Count; i++)
                {
                    defs.Children[i] = defs.Children[i].Select(func) as ContentPart;
                }
            }

            if (newDef is IContent content)
            {
                for (int i = 0; i < content.Inlines.Count; i++)
                {
                    content.Inlines[i] = content.Inlines[i].Select(func) as Inline;
                }
            }

            if (newDef is Span span)
            {
                for (int i = 0; i < span.Inlines.Count; i++)
                {
                    span.Inlines[i] = span.Inlines[i].Select(func) as Inline;
                }
            }

            func(newDef);

            return newDef;
        }

        public static void DoForAllTextDeep(this IDefinition def, Action<Run> func)
        {
            void funcInlines(InlineCollection inlines)
            {
                foreach (var inline in inlines)
                    funcInline(inline);
            }

            void funcInline(Inline inline)
            {
                if (def is Run run)
                    func(run);
                else if (def is Span span)
                    funcInlines(span.Inlines);
            }

            if (def is Inline inline)
                funcInline(inline);
            
            if (def is IContent content)
                funcInlines(content.Inlines);

            if (def is Section section && section.Title != null)
                funcInlines(section.Title.Inlines);
        }
    }
}
