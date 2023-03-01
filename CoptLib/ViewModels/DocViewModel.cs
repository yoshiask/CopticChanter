using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
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
        Layout = layout;
        Doc = layout.Doc;
        CreateTableCommand = new AsyncRelayCommand(() => Task.Run(CreateTable));
    }

    [ObservableProperty]
    private Doc _doc;

    [ObservableProperty]
    DocLayout _layout;

    [ObservableProperty]
    private IAsyncRelayCommand _createTableCommand;

    public List<List<object>> CreateTable()
    {
        // Apply transforms before display
        Doc.ApplyTransforms();

        return Layout.CreateTable();
    }
}
