using CoptLib.Models;
using System;

namespace CoptLib.Extensions
{
    public static class DefinitionExtensions
    {
        public static IDefinition Select(this IDefinition rootDef, Action<IDefinition> func)
        {
            var newDef = rootDef.Copy();
            if (newDef is IContentCollectionContainer defs)
            {
                foreach (var def in defs.Children)
                    func(def);
            }
            else
            {
                func(newDef);
            }
            return newDef;
        }
    }
}
