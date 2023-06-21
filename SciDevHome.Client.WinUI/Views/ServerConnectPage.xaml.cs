using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class ServerConnectPage : Page
{
    public ServerConnectViewModel ViewModel
    {
        get;
    }

    public ServerConnectPage()
    {
        ViewModel = App.GetService<ServerConnectViewModel>();
        InitializeComponent();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.IsNotSearch = false;

        await ViewModel.UpdateServer();
        ViewModel.IsNotSearch = true;

    }
}
