using CommunityToolkit.Mvvm.ComponentModel;

using SciDevHome.Client.WinUI.Contracts.ViewModels;
using SciDevHome.Client.WinUI.Core.Contracts.Services;
using SciDevHome.Client.WinUI.Core.Models;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class SoftwareDownloadDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    [ObservableProperty]
    private SampleOrder? item;

    public SoftwareDownloadDetailViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is long orderID)
        {
            var data = await _sampleDataService.GetContentGridDataAsync();
            Item = data.First(i => i.OrderID == orderID);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
