using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class TransliterateCmd : TextCommandBase
    {
        public TransliterateCmd(string cmd, IContent content, int startIndex, IDefinition[] parameters)
            : base(cmd, content, startIndex, parameters)
        {
            Parse(cmd, parameters);
        }

        public LanguageInfo Language { get; private set; }

        private void Parse(string cmd, params IDefinition[] parameters)
        {
            var langParam = ((IContent)parameters[0]).SourceText;
            var sourceParam = parameters[parameters.Length - 1];

            Language = LanguageInfo.Parse(langParam)
                ?? throw new ArgumentException($"Unknown language '{langParam}' in {nameof(TransliterateCmd)}");

            Output = sourceParam.Select(def =>
            {
                if (def is IContent content)
                    content.Text = CopticInterpreter.Transliterate(content.Text ?? content.SourceText, Language.Known);

                if (def is IMultilingual multi)
                {
                    if (multi.Language != null)
                    {
                        // Set secondary language to indicate transliteration
                        multi.Language.Secondary = Language;
                    }
                    else
                    {
                        multi.Language = Language;
                    }
                }
            });
        }
    }
}
