using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class LanguageCmd : TextCommandBase
    {
        public LanguageCmd(string cmd, IContent content, int startIndex, IDefinition[] parameters)
            : base(cmd, content, startIndex, parameters)
        {
            Parse(cmd, parameters);
        }

        public KnownLanguage Language { get; private set; }

        public CopticFont Font { get; private set; }

        private void Parse(string cmd, params IDefinition[] parameters)
        {
            var langParam = ((IContent)parameters[0]).SourceText;
            var sourceParam = parameters[parameters.Length - 1];

            if (!Enum.TryParse<KnownLanguage>(langParam, out var language))
                return;

            Language = language;

            if (parameters.Length >= 3 && (language == KnownLanguage.Coptic || language == KnownLanguage.Greek))
            {
                var fontParam = ((IContent)parameters[1]).SourceText;

                Font = CopticFont.FindFont(fontParam) ?? CopticFont.CsAvvaShenouda;
            }

            Output = sourceParam.Select(def =>
            {
                if (def is IContent content)
                    content.Text = Font != null ? Font.Convert(content.SourceText) : content.SourceText;
            });
        }
    }
}
