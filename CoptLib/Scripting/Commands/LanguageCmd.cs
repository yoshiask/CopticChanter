using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class LanguageCmd : TextCommandBase
    {
        public LanguageCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
            : base(cmd, inline, parameters)
        {
            Parse();
        }

        public LanguageInfo Language { get; private set; }

        public CopticFont Font { get; private set; }

        private void Parse()
        {
            var langParam = Parameters[0].ToString();
            var sourceParam = Parameters[Parameters.Length - 1];

            if (!LanguageInfo.TryParse(langParam, out var language))
                return;

            Language = language;

            if (Parameters.Length >= 3 && (language.Known == KnownLanguage.Coptic || language.Known == KnownLanguage.Greek))
            {
                var fontParam = Parameters[1].ToString();

                Font = CopticFont.FindFont(fontParam) ?? CopticFont.CsAvvaShenouda;
            }

            Output = sourceParam.Select(ConvertFont);
            Evaluated = true;
        }

        private void ConvertFont(IDefinition def)
        {
            if (Font != null && def is Run run)
                run.Text = Font.Convert(run.Text);

            if (def is IMultilingual multi)
            {
                // Clear the font since we've already handled it here
                multi.Font = null;

                // Set the new language
                multi.Language = Language;
            }
        }
    }
}
