using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class ClientListPage : Page
{
    public ClientListViewModel ViewModel
    {
        get;
    }

    public ClientListPage()
    {
        ViewModel = App.GetService<ClientListViewModel>();
        InitializeComponent();
    }
}
