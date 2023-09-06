using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Sequences;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoptLib.ViewModels;

public partial class DocSetViewModel : ObservableObject
{
    public DocSetViewModel(string? name, IEnumerable<Doc> docs)
    {
        _createTablesCommand = new AsyncRelayCommand(CreateTablesAync);
        _docs = new(docs.Select(d => new DocViewModel(d)));
        Name = name;
    }

    public DocSetViewModel(DocSet set) : this(set.Name, set.IncludedDocs)
    {
    }

    public DocSetViewModel(Sequence sequence) : this(sequence.Name, sequence.EnumerateDocs().ToEnumerable())
    {

    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private ObservableCollection<List<List<object>>> _tables = new();

    [ObservableProperty]
    private ObservableCollection<DocViewModel> _docs;

    [ObservableProperty]
    private IAsyncRelayCommand _createTablesCommand;

    /// <summary>
    /// Creates a new <see cref="DocSet"/> view model from the given folder.
    /// </summary>
    /// <param name="folder">
    /// The root folder of the doc set.
    /// </param>
    /// <param name="context"></param>
    /// <returns>
    /// A <see cref="DocSetViewModel"/> representing the set.
    /// </returns>
    public static async Task<DocSetViewModel> LoadFromFolder(IFolder folder, LoadContextBase? context = null)
    {
        DocSetReader reader = new(folder, context);
        await reader.ReadDocs();
        return new(reader.Set);
    }

    public async Task CreateTablesAync(CancellationToken token = default)
    {
        Tables.Clear();

        foreach (var dvm in Docs)
        {
            var table = await dvm.CreateTableAsync(token);
            token.ThrowIfCancellationRequested();

            // Ignore layouts that are empty (or just a Doc with no content)
            if (table.Count <= 1)
                continue;

            Tables.Add(table);
        }
    }
}
