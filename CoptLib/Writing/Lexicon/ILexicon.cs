﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoptLib.Writing.Lexicon;

/// <summary>
/// An interface that represents a lexicon.
/// </summary>
public interface ILexicon
{
    LanguageInfo Language { get; }

    IAsyncEnumerable<LexiconEntry> SearchAsync(string query, LanguageInfo usage, PartOfSpeech? partOfSpeech = null, CancellationToken token = default);

    IAsyncEnumerable<ILexiconEntry> GetEntriesAsync(CancellationToken token = default);

    Task<ILexiconEntry> GetEntryAsync(string id, CancellationToken token = default);
}
