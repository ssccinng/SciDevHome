using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Grpc.Net.ClientFactory;
using SciDevHome.Data;
using SciDevHome.Server;
using SciDevHome.Utils;
using Windows.Media.Protection.PlayReady;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class DirctoryPathViewViewModel : ObservableRecipient
{
    [ObservableProperty]
   ObservableCollection<ClientItem> _clientInfos = new ();
    [ObservableProperty]
    ObservableCollection<Folder> _nowFoldList = new();

    private readonly GrpcClientFactory _grpcClientFactory;
    Greeter.GreeterClient _client;
    
    /// <summary>
    /// 当前选中的客户端
    /// </summary>
    [ObservableProperty]
    ClientItem _selectClient;


    [ObservableProperty] private ObservableCollection<string> _baseFolderPath = new();

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

        // 能直接赋值吗 Todo: 缓存之前的路径
        BaseFolderPath.Clear();
        BaseFolderPath.Add("/");
        // 要获取客户端的嗼
        // RefreshFolder(ZQDHelper.GetRootPath());
        // 先尝试获取根，后面再考虑获取存档之中
        GetPath("");

    }
    /// <summary>
    /// 刷新当前目录
    /// </summary>
    /// <param name="folders"></param>
    private void RefreshFolder(IEnumerable<Folder> folders)
    {
        NowFoldList.Clear();
        foreach (var item in folders )
        {
            NowFoldList.Add(item);
        }
    }


    internal async void GetPath(string name)
    {
        // 需要完整路径des
        if (SelectClient == null) return;
        var path = await _client.GetClientPathAsync(new SciDevHome.Server.GetPathRequest { ClientId = SelectClient.ClientId, Path = name });
        RefreshFolder(path.Files.Select(s => new Folder { Name = s.Name, IsDirectory = s.IsDirectory }));
    }
    
    internal async void GetPathFilename(string name)
    {
        // 需要完整路径des
        if (SelectClient == null) return;
        var path = await _client.GetClientPathAsync(new SciDevHome.Server.GetPathRequest
        {
            // 优化
            ClientId = SelectClient.ClientId, Path = string.Join("/", BaseFolderPath.Skip(1))
        });
        RefreshFolder(path.Files.Select(s => new Folder
        {
            Name = BaseFolderPath.Count == 1 ? s.Name : Path.GetFileName(s.Name),
            IsDirectory = s.IsDirectory
        }));
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
