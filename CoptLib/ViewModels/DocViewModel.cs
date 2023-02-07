using CommunityToolkit.Mvvm.ComponentModel;
using CoptLib.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace CoptLib.ViewModels;

public partial class DocViewModel : ObservableObject
{
    public DocViewModel(Doc doc)
    {
        Doc = doc;
    }

    [ObservableProperty]
    private Doc _doc;

    [ObservableProperty]
    ObservableCollection<ObservableCollection<object>> _layout;

    public void ApplyTransforms() => Doc.ApplyTransforms();

    public ObservableCollection<ObservableCollection<object>> CreateLayout()
    {
        // Apply transforms before display
        Doc.ApplyTransforms();

        Layout = new(Doc
            .Flatten()
            .Select(r => new ObservableCollection<object>(r))
        );
        return Layout;
    }
}
