using CommunityToolkit.WinUI.UI.Animations;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using SciDevHome.Client.WinUI.Contracts.Services;
using SciDevHome.Client.WinUI.ViewModels;

namespace SciDevHome.Client.WinUI.Views;

public sealed partial class SoftwareDownloadDetailPage : Page
{
    public SoftwareDownloadDetailViewModel ViewModel
    {
        get;
    }

    public SoftwareDownloadDetailPage()
    {
        ViewModel = App.GetService<SoftwareDownloadDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            var navigationService = App.GetService<INavigationService>();

            if (ViewModel.Item != null)
            {
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }
}
