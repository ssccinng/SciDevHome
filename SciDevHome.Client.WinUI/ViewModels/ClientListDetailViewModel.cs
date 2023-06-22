using CommunityToolkit.Mvvm.ComponentModel;

using SciDevHome.Client.WinUI.Contracts.ViewModels;
using SciDevHome.Client.WinUI.Core.Contracts.Services;
using SciDevHome.Client.WinUI.Core.Models;
using SciDevHome.Client.WinUI.Services;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class ClientListDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;
    private readonly DevHomeClientService _devHomeClientService;

    [ObservableProperty]
    private ClientItem? item;

    public ClientListDetailViewModel(ISampleDataService sampleDataService, DevHomeClientService devHomeClientService)
    {
        _sampleDataService = sampleDataService;
        _devHomeClientService = devHomeClientService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is string orderID)
        {
            //var data = await _devHomeClientService.GetClientDataAsync(parameter);
            // Item = data.First(i => i.OrderID == orderID);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
