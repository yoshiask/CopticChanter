using CoptLib.Models;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class LanguageCmd : TextCommandBase
    {
        public LanguageCmd(string cmd, Doc context, int startIndex, string[] parameters)
            : base(cmd, context, startIndex, parameters)
        {
            Parse(cmd, parameters);
        }

        public Language Language { get; private set; }

        public CopticFont Font { get; private set; }

        private void Parse(string cmd, params string[] parameters)
        {
            Text = parameters[1];

            string[] langParts = parameters[0].Split(':');
            if (!Enum.TryParse<Language>(langParts[0], out var language))
                return;

            Language = language;

            switch (Language)
            {
                case Language.Coptic:
                    if (langParts.Length >= 2)
                    {
                        Font = CopticFont.FindFont(langParts[1]) ?? CopticFont.CsAvvaShenouda;
                        Text = Font.Convert(Text);
                    }
                    break;
            }
        }
    }
}
