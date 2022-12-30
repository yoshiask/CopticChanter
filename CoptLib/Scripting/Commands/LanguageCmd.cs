using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class LanguageCmd : TextCommandBase
    {
        public LanguageCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
            : base(cmd, inline, parameters)
        {
            Parse();
        }

        public KnownLanguage Language { get; private set; }

        public CopticFont Font { get; private set; }

        private void Parse()
        {
            var langParam = Parameters[0].ToString();
            var sourceParam = Parameters[Parameters.Length - 1];

            if (!Enum.TryParse<KnownLanguage>(langParam, out var language))
                return;

            Language = language;

            if (Parameters.Length >= 3 && (language == KnownLanguage.Coptic || language == KnownLanguage.Greek))
            {
                var fontParam = Parameters[1].ToString();

                Font = CopticFont.FindFont(fontParam) ?? CopticFont.CsAvvaShenouda;
            }

            Output = sourceParam.Select(ConvertFont);
        }

        private void ConvertFont(IDefinition def)
        {
            if (Font != null && def is Run run)
                run.Text = Font.Convert(run.Text);
        }
    }
}
