namespace CoptLib.Writing.Linguistics.Analyzers;

public class GreekAnalyzer : CopticGrecoBohairicAnalyzer
{
    public GreekAnalyzer(LanguageInfo language) : base(language)
    {
    }

    public GreekAnalyzer() : this(new LanguageInfo(KnownLanguage.Greek))
    {
    }
    
    public override PhoneticWord[] PhoneticAnalysis(string srcText)
    {
        string copText = CopticFont.GreekUnicode.Convert(srcText, CopticFont.CopticUnicode);
        copText = copText.Replace('ϣ', 'ⲥ');

        var text = base.PhoneticAnalysis(copText);
        
        for (int i = 0; i < text.Length; ++i)
        {
            var word = text[i];
            for (int j = 0; j < word.Equivalents.Count; ++j)
            {
                var pe = word.Equivalents[j];
                pe.Source = CopticFont.CopticUnicode.Convert(pe.Source.ToString(), CopticFont.GreekUnicode)[0];

                if (j == word.Equivalents.Count - 1 && pe.Source == 'σ')
                    pe.Source = 'ς';
            }
        }

        return text;
    }
}