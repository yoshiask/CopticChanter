using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class LanguageCmd : TextCommandBase
    {
        public LanguageCmd(string cmd, Run run, IDefinition[] parameters)
            : base(cmd, run, parameters)
        {
            Parse(cmd, parameters);
        }

        public KnownLanguage Language { get; private set; }

        public CopticFont Font { get; private set; }

        private void Parse(string cmd, params IDefinition[] parameters)
        {
            var langParam = parameters[0].ToString();
            var sourceParam = parameters[parameters.Length - 1];

            if (!Enum.TryParse<KnownLanguage>(langParam, out var language))
                return;

            Language = language;

            if (parameters.Length >= 3 && (language == KnownLanguage.Coptic || language == KnownLanguage.Greek))
            {
                var fontParam = parameters[1].ToString();

                Font = CopticFont.FindFont(fontParam) ?? CopticFont.CsAvvaShenouda;
            }

            Output = sourceParam.Select(def => def.DoForAllTextDeep(ConvertFont));
        }

        private void ConvertFont(Run run)
        {
            if (Font != null)
                run.Text = Font.Convert(run.Text);
        }
    }
}
