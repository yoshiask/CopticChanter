using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoptLib.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoptLib.ViewModels;

public partial class DocViewModel : ObservableObject
{
    public DocViewModel(Doc doc) : this(new DocLayout(doc))
    {
    }

    public DocViewModel(DocLayout layout)
    {
        _layout = layout;
        _doc = layout.Doc;
        _createTableCommand = new AsyncRelayCommand(() => Task.Run(CreateTable));
    }

    [ObservableProperty]
    private Doc _doc;

    [ObservableProperty]
    private DocLayout _layout;

    [ObservableProperty]
    private IAsyncRelayCommand _createTableCommand;

    public List<List<object>> CreateTable()
    {
        // Apply transforms before display
        Doc.ApplyTransforms();

        return Layout.CreateTable();
    }
}
