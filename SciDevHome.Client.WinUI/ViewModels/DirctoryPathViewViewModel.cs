using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Grpc.Net.ClientFactory;
using SciDevHome.Server;
using Windows.Media.Protection.PlayReady;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class DirctoryPathViewViewModel : ObservableRecipient
{
    [ObservableProperty]
   ObservableCollection<ClientItem> _clientInfos = new ();


    private readonly GrpcClientFactory _grpcClientFactory;
    Greeter.GreeterClient _client;
    public DirctoryPathViewViewModel(GrpcClientFactory grpcClientFactory)
    {

        _grpcClientFactory = grpcClientFactory;
        _client = _grpcClientFactory.CreateClient<Greeter.GreeterClient>("test");
        //ClientInfos.Add(new ClientItem() { ClientId = "12312414", Name = "zqd" });
        //ClientInfos.Add(new ClientItem() { ClientId = "12312414", Name = "临流" });
    }

    internal void RefreshClient()
    {
        var clients = _client.GetClients(new GetClientsRequest());
        ClientInfos.Clear();
       var ff  = new ObservableCollection<ClientItem>(clients.Clients.Select(client => new ClientItem { ClientId = client.ClientId, Name = client.Name }));
        foreach (var client in ff) { ClientInfos.Add(client); }
    }
}

public partial class ClientItem : ObservableRecipient
{
    [ObservableProperty]
    [NotifyPropertyChangedFor("ClientName")]
    string _name;
    [ObservableProperty]
    [NotifyPropertyChangedFor("ClientName")]
    string _clientId;


    public string ClientName => $"{Name}({ClientId})";
}
