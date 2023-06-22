using Grpc.Net.Client;
using SciDevHome.Providers;
using SciDevHome.Server;

namespace SciDevHome.API;

public class ConnectAPIFactory
{
    
}
public class ConnectAPI
{
    // 如何自在化（？？
    public ConnectAPI(Greeter.GreeterClient client)
    {
        // 初始化grpc(? howto
    }

    private Greeter.GreeterClient _client;
    // 只需在初始化的时候就获取服务然后初始化（？？？ 但若是不止一个怎么办
    // 那就工厂！！！
    public void Init()
    {
        
        
    }
    
    // 需要这个t吗
    public async Task<ConnectResponse> SendRequestAsync<T>(
        ConnectProvider<T> provide, T Data)
    {
        throw new NotImplementedException();
    }
}