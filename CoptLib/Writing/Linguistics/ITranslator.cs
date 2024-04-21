using System;
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
        if (!LinguisticLanguageService.TryIdentifyLanguage(sourceText, out var sourceLanguage))
            throw new ArgumentException($"Unable to identify language of source text: '{sourceText}'");

        return translator.TranslateAsync(sourceText, sourceLanguage, targetLanguage);
    }
}
