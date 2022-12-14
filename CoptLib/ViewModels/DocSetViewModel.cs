using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoptLib.IO;
using CoptLib.Models;
using OwlCore.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoptLib.ViewModels;

public partial class DocSetViewModel : ObservableObject
{
    private DocSetViewModel()
    {
        CreateLayoutCommand = new AsyncRelayCommand(() => Task.Run(CreateLayout));
    }

    public DocSetViewModel(DocSet set) : this()
    {
        Set = set;
        Docs = new(set.IncludedDocs.Select(d => new DocViewModel(d)));
    }

    public DocSetViewModel(IEnumerable<Doc> docs) : this()
    {
        Docs = new(docs.Select(d => new DocViewModel(d)));
    }

    [ObservableProperty]
    private DocSet _set;

    [ObservableProperty]
    private ObservableCollection<ObservableCollection<ObservableCollection<object>>> _layout = new();

    [ObservableProperty]
    private ObservableCollection<DocViewModel> _docs;

    [ObservableProperty]
    private IAsyncRelayCommand _createLayoutCommand;

    /// <summary>
    /// Creates a new <see cref="DocSet"/> view model from the given file.
    /// </summary>
    /// <param name="file">
    /// The doc set file to read.
    /// </param>
    /// <returns>
    /// A <see cref="DocSetViewModel"/> representing the set.
    /// </returns>
    public static async Task<DocSetViewModel> CreateFromFile(IFile file)
    {
        using var fileStream = await file.OpenStreamAsync(System.IO.FileAccess.Read);
        using DocSetReader reader = new(fileStream);
        reader.ReadAll();
        return new(reader.Set);
    }

    public void CreateLayout()
    {
        Layout.Clear();

        foreach (var layout in Docs.Select(dvm => dvm.CreateLayout()))
        {
            // Ignore layouts that are empty (or just a Doc with no content)
            if (layout.Count <= 1)
                continue;

            Layout.Add(layout);
        }
    }
}
