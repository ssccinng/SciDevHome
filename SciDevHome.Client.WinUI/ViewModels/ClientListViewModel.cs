using System.Collections.ObjectModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SciDevHome.Client.WinUI.Contracts.Services;
using SciDevHome.Client.WinUI.Contracts.ViewModels;
using SciDevHome.Client.WinUI.Core.Contracts.Services;
using SciDevHome.Client.WinUI.Core.Models;
using SciDevHome.Client.WinUI.Services;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class ClientListViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly ISampleDataService _sampleDataService;
    private readonly DevHomeClientService _devHomeClientService;

    public ObservableCollection<ClientItem> Source { get; } = new();

    public ClientListViewModel(INavigationService navigationService, ISampleDataService sampleDataService, DevHomeClientService devHomeClientService)
    {
        _navigationService = navigationService;
        _sampleDataService = sampleDataService;
        _devHomeClientService = devHomeClientService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        // var data = await _sampleDataService.GetContentGridDataAsync();
        var data = await _devHomeClientService.GetClientsAsync();
        foreach (var item in data)
        {
            // 当跳转到此时 的时候
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
        // 这个是（？
    }

    [RelayCommand]
    private void OnItemClick(ClientItem? clickedItem)
    {
        if (clickedItem != null)
        {
            // return;
            // 这是个动画吗
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ClientListDetailViewModel).FullName!, clickedItem.ClientId);
        }
    }
}
