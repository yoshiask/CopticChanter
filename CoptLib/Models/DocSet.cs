using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Models
{
    public class DocSet
    {
        public DocSet(string uuid, string name, IEnumerable<Doc> docs = null)
        {
            Uuid = uuid;
            Name = name;
            IncludedDocs = docs?.ToList() ?? new();
        }

        public string Uuid { get; set; }

        public string Name { get; set; }

        public List<Doc> IncludedDocs { get; }

        public Author Author { get; set; }
    }
}
