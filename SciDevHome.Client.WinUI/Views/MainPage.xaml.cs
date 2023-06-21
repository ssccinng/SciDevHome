using System.Text.Json;
using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;
using SciDevHome.Message;
using SciDevHome.Utils;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
 
    private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
       // await Init();
    }
}
