using CoptLib.Models;
using CoptLib.Writing.Linguistics.XBar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoptLib.Writing.Linguistics;

public interface ITranslator
{
    Task SetSourceLanguageAsync(LanguageInfo language);
    Task<XBarNode> TranslateAsync(IAsyncEnumerable<IStructuralElement> annotatedText);
    IAsyncEnumerable<IAsyncEnumerable<IEnumerable<IStructuralElement>>> AnnotateAsync(string sourceText);
}

public static class TranslatorExtensions
{
    public static async Task<XBarNode> TranslateAsync(this ITranslator translator,
        string sourceText, LanguageInfo sourceLanguage)
    {
        var annotatedText = translator.AnnotateAsync(sourceText);
        throw new NotImplementedException();
        //return await translator.TranslateAsync(annotatedText, sourceLanguage);
    }

    public static Task<XBarNode> TranslateAsync<TSource>(this ITranslator translator,
        TSource source) where TSource : IMultilingual, IContent
    {
        var sourceText = source.GetText();
        return translator.TranslateAsync(sourceText, source.Language);
    }
}
