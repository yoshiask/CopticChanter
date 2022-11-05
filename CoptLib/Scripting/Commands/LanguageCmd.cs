using CoptLib.Models;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class LanguageCmd : TextCommandBase
    {
        public LanguageCmd(string cmd, IContent content, Doc context, int startIndex, string[] parameters)
            : base(cmd, content, context, startIndex, parameters)
        {
            Parse(cmd, parameters);
        }

        public Language Language { get; private set; }

        public CopticFont Font { get; private set; }

        private void Parse(string cmd, params string[] parameters)
        {
            Text = parameters[parameters.Length - 1];

            if (!Enum.TryParse<Language>(parameters[0], out var language))
                return;

            Language = language;

            switch (Language)
            {
                case Language.Coptic:
                    if (parameters.Length >= 3)
                    {
                        Font = CopticFont.FindFont(parameters[1]) ?? CopticFont.CsAvvaShenouda;
                        Text = Font.Convert(Text);
                    }
                    break;
            }
        }
    }
}
