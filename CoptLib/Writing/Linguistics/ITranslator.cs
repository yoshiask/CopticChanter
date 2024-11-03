using CoptLib.Models;
using CoptLib.Writing.Linguistics.XBar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoptLib.Writing.Linguistics;

public interface ITranslator
{
    Task SetSourceLanguageAsync(LanguageInfo language);
    Task<BinaryNode<IStructuralElement>> TranslateAsync(IAsyncEnumerable<IStructuralElement> annotatedText);
    IAsyncEnumerable<List<IStructuralElement>> AnnotateAsync(string sourceText);
}

public static class TranslatorExtensions
{
    public static async Task<BinaryNode<IStructuralElement>> TranslateAsync(this ITranslator translator,
        string sourceText, LanguageInfo sourceLanguage)
    {
        var annotatedText = translator.AnnotateAsync(sourceText);
        throw new NotImplementedException();
        //return await translator.TranslateAsync(annotatedText, sourceLanguage);
    }

    public static Task<BinaryNode<IStructuralElement>> TranslateAsync<TSource>(this ITranslator translator,
        TSource source) where TSource : IMultilingual, IContent
    {
        var sourceText = source.GetText();
        return translator.TranslateAsync(sourceText, source.Language);
    }
}
