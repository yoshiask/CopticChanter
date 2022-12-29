using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using System;

namespace CoptLib.Scripting.Commands
{
    public class TransliterateCmd : TextCommandBase
    {
        public TransliterateCmd(string cmd, Run run, IDefinition[] parameters)
            : base(cmd, run, parameters)
        {
            Parse(cmd, parameters);
        }

        public LanguageInfo Language { get; private set; }

        private void Parse(string cmd, params IDefinition[] parameters)
        {
            var langParam = parameters[0].ToString();
            var sourceParam = parameters[parameters.Length - 1];

            Language = LanguageInfo.Parse(langParam)
                ?? throw new ArgumentException($"Unknown language '{langParam}' in {nameof(TransliterateCmd)}");

            Output = sourceParam.Select(def => def.DoForAllTextDeep(Transliterate));

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

        private void Transliterate(Run run)
        {
            run.Text = CopticInterpreter.Transliterate(run.Text, Language.Known);
            run.Font = null;
        }
    }
}
