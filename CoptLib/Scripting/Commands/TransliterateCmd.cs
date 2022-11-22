using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoptLib.Scripting.Commands
{
    public class TransliterateCmd : TextCommandBase
    {
        public TransliterateCmd(string cmd, IContent content, int startIndex, IDefinition[] parameters)
            : base(cmd, content, startIndex, parameters)
        {
            Parse(cmd, parameters);
        }

        public Language Language { get; private set; }

        private void Parse(string cmd, params IDefinition[] parameters)
        {
            var langParam = ((IContent)parameters[0]).SourceText;
            var sourceParam = parameters[parameters.Length - 1];

            if (!Enum.TryParse<Language>(langParam, out var language))
                throw new ArgumentException($"Unknown language '{langParam}' in {nameof(TransliterateCmd)}");

            Language = language;

            Output = sourceParam.Select(def =>
            {
                if (def is IContent content)
                    content.Text = CopticInterpreter.Transliterate(content.Text ?? content.SourceText, language);
            });
        }
    }
}
