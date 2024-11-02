using System;

namespace CoptLib.Writing.Linguistics.XBar;

public record SemanticPair(Pattern Pattern, Func<IMeta> MetaFactory);
