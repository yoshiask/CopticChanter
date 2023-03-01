using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoptLib.IO;
using CoptLib.Models;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoptLib.ViewModels;

public partial class DocSetViewModel : ObservableObject
{
    private DocSetViewModel()
    {
        CreateTableCommand = new AsyncRelayCommand(() => Task.Run(CreateTable));
    }

    public DocSetViewModel(DocSet set) : this(set.IncludedDocs)
    {
        Set = set;
    }

    public DocSetViewModel(IEnumerable<Doc> docs) : this()
    {
        Docs = new(docs.Select(d => new DocViewModel(d)));
    }

    [ObservableProperty]
    private DocSet _set;

    [ObservableProperty]
    private ObservableCollection<List<List<object>>> _table = new();

    [ObservableProperty]
    private ObservableCollection<DocViewModel> _docs;

    [ObservableProperty]
    private IAsyncRelayCommand _createTableCommand;

    /// <summary>
    /// Creates a new <see cref="DocSet"/> view model from the given folder.
    /// </summary>
    /// <param name="folder">
    /// The root folder of the doc set.
    /// </param>
    /// <returns>
    /// A <see cref="DocSetViewModel"/> representing the set.
    /// </returns>
    public static async Task<DocSetViewModel> ReadFromFile(IFolder folder, LoadContextBase context = null)
    {
        DocSetReader reader = new(folder, context);
        await reader.ReadAll();
        return new(reader.Set);
    }

    public void CreateTable()
    {
        Table.Clear();

        foreach (var table in Docs.Select(dvm => dvm.CreateTable()))
        {
            // Ignore layouts that are empty (or just a Doc with no content)
            if (table.Count <= 1)
                continue;

            Table.Add(table);
        }
    }
}
