using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class TransliterateCmd : TextCommandBase
    {
        public TransliterateCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
            : base(cmd, inline, parameters)
        {
            Parse();
        }

        public LanguageInfo Language { get; private set; }

        private void Parse()
        {
            var langParam = Parameters[0].ToString();
            var sourceParam = Parameters[Parameters.Length - 1];

            Language = LanguageInfo.Parse(langParam)
                ?? throw new ArgumentException($"Unknown language '{langParam}' in {nameof(TransliterateCmd)}");

            Output = sourceParam.Select(Transliterate);

            if (Output is IMultilingual multi)
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

                multi.Font = null;
            }
        }

        private void Transliterate(IDefinition def)
        {
            if (def is Run run)
            {
                run.Text = CopticInterpreter.Transliterate(run.Text, Language.Known);
                run.Font = null;
            }

            // Make sure referenced elements are also transliterated
            else if (def is InlineCommand inCmd && inCmd.Command.Output != null)
                inCmd.Command.Output = inCmd.Command.Output.Select(Transliterate);
        }
    }
}
