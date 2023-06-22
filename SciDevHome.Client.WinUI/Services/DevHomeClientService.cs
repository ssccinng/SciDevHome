using Grpc.Net.ClientFactory;
using SciDevHome.Client.WinUI.ViewModels;
using SciDevHome.Server;

namespace SciDevHome.Client.WinUI.Services;

public class DevHomeClientService
{
    private readonly GrpcClientFactory _grpcClientFactory;
    private readonly Greeter.GreeterClient _client;

    public IEnumerable<ClientItem> ClientItems
    {
        get;
        private set;
    } = Array.Empty<ClientItem>();

    public DevHomeClientService(GrpcClientFactory grpcClientFactory)
    {
        _grpcClientFactory = grpcClientFactory;
        // 这个test可能得根据实际连接的选择
        _client = _grpcClientFactory.CreateClient<Greeter.GreeterClient>("test");
    }
        
    /// <summary>
    /// 获取所有客户端.. 是不是应该放到中介者？
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ClientItem>> GetClientsAsync()
    {
        if (!ClientItems.Any())
        {
            await RefreshClient();
        }

        return ClientItems;
    }

    public async Task RefreshClient()
    {
        var clients = await _client.GetClientsAsync(new GetClientsRequest());

        ClientItems =
            clients.Clients.Select(client => new ClientItem { ClientId = client.ClientId, Name = client.Name });
    }
    /// <summary>
    /// 获取客户端数据（？？
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<object> GetClientDataAsync(object parameter)
    {
        throw new NotImplementedException();
    }
}