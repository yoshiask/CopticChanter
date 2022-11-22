using CoptLib.Models;
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
            else
            {
                func(newDef);
            }
            return newDef;
        }
    }
}
