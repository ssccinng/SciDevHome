using System.Text.Json;
using Grpc.Net.Client;
using SciDevHome.Providers;
using SciDevHome.Server;

namespace SciDevHome;

public class ConnectAPIFactory
{
    
}
public class ConnectAPI
{
    // 如何自在化（？？
    // public ConnectAPI(Greeter.GreeterClient client)
    // {
    //     // 初始化grpc(? howto
    // }
    //
    // private Greeter.GreeterClient _client;
    // 只需在初始化的时候就获取服务然后初始化（？？？ 但若是不止一个怎么办
    // 那就工厂！！！
    //public void Init()
    //{
        
        
    //}
    
    // 需要这个t吗
    // 这更像是回复吧
    public static async Task<ConnectRequest> SendReplyAsync(
        ConnectProvider provide, ConnectRequest Data) // 这里的t有用吗
    {
        var res =  provide.Handler(Data.Data);
        return new ConnectRequest
        {
            Cmd = "Reply",
            Data = JsonSerializer.Serialize(res),
            ReqId = Data.ReqId
        };
        // 判断data的
        // 这里如何传入拿什么流
        // 我想想（？用作扩展方法

    }
    // 构建请求

}