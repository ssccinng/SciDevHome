using Microsoft.UI.Xaml.Controls;

using SciDevHome.Client.WinUI.ViewModels;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class SoftwareDownloadPage : Page
{
    public SoftwareDownloadViewModel ViewModel
    {
        get;
    }

    public SoftwareDownloadPage()
    {
        ViewModel = App.GetService<SoftwareDownloadViewModel>();
        InitializeComponent();
    }
}
