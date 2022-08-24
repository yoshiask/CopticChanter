using CoptLib.Models;
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
                        Font = CopticFont.Fonts.Find(f => f.Name.ToLower() == langParts[1].ToLower()) ?? CopticFont.CsAvvaShenouda;
                        Text = CopticInterpreter.ConvertFont(Text, Font, CopticFont.CopticUnicode);
                    }

                    // TextBlock doesn't seem to know where to break Coptic (Unicode?)
                    // lines, so insert a zero-width space at every space so
                    // word wrap actually works
                    Text = Text.Replace(" ", " \u200B");

                    break;
            }
        }
    }
}
