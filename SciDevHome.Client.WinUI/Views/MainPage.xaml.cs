using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;

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
}
