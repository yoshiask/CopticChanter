using System.Collections.Generic;

namespace CoptLib.Writing.Linguistics.XBar;

public interface IMeta;

public record CompoundMeta(List<IMeta> Metas) : IMeta;
