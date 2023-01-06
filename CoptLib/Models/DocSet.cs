using CommunityToolkit.Diagnostics;
using CoptLib.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoptLib.Models
{
    public class DocSet : IContextualLoad
    {
        private LoadContext _context;

        public DocSet(string uuid, string name, IEnumerable<Doc> docs = null, LoadContext context = null)
        {
            Uuid = uuid;
            Name = name;
            IncludedDocs = docs?.ToList() ?? new();
            Context = context ?? new();
        }

        public string Uuid { get; set; }

        public string Name { get; set; }

        public List<Doc> IncludedDocs { get; }

        public Author Author { get; set; }

        [NotNull]
        public LoadContext Context
        {
            get => _context;
            set
            {
                Guard.IsNotNull(value);
                _context = value;
            }
        }
    }
}
