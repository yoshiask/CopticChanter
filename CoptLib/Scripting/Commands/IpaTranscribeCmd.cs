using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using CoptLib.Writing.Linguistics;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase
    {
        private static readonly LanguageInfo _ipaLang = new(KnownLanguage.IPA);

        public IpaTranscribeCmd(string name, InlineCommand inline, IDefinition[] parameters)
            : base(name, inline, parameters)
        {
            Parse();
        }

        public IDefinition Source { get; private set; }

        public LinguisticAnalyzer Analyzer { get; private set; }

        private void Parse()
        {
            Source = Parameters[0];
            Analyzer = LinguisticLanguageService.Default.GetAnalyzerForLanguage(Source.GetLanguage());

            Output = Source.Select(Transcribe);

            Evaluated = true;
        }

        private void Transcribe(IDefinition def)
        {
            if (def is Run run)
                run.Text = Analyzer.IpaTranscribe(run.Text);

            if (def is IMultilingual multi)
            {
                // Ensure that the language and font are set.
                // Set secondary language to indicate transliteration.
                if (multi.Language != null)
                    multi.Language.Secondary = _ipaLang;
                else
                    multi.Language = _ipaLang;

                multi.Font = null;
            }

            // Make sure referenced elements are also transliterated
            if (def is InlineCommand inCmd && inCmd.Command.Output != null)
                inCmd.Command.Output = inCmd.Command.Output.Select(Transcribe);
        }
    }
}
