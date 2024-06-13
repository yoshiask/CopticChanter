using System.Threading.Tasks;

namespace CoptLib.Writing.Linguistics;

public interface ITranslator
{
    Task<string> TranslateAsync(string sourceText, LanguageInfo sourceLanguage, LanguageInfo targetLanguage);
}

public static class TranslatorExtensions
{
    public static Task<string> TranslateAsync(this ITranslator translator, string sourceText, LanguageInfo targetLanguage)
    {
        var sourceLanguage = LinguisticLanguageService.IdentifyLanguage(sourceText);

        return translator.TranslateAsync(sourceText, sourceLanguage, targetLanguage);
    }
}
