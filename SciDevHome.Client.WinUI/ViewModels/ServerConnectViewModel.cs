using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SciDevHome.Utils;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class ServerConnectViewModel : ObservableRecipient
{
    [ObservableProperty]
    ObservableCollection<string> _servers = new();

    [ObservableProperty] private bool _isNotSearch = true;
    public ServerConnectViewModel()
    {
    }

    public async Task UpdateServer()
    {
        var ss = await ZQDHelper.GetServerListAsync();

        Servers.Clear();

        foreach (var item in ss)
        {
            Servers.Add(item);
        }

    }
}
