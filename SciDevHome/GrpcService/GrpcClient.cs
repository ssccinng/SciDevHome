using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Net.Client;
using SciDevHome.Server;

namespace SciDevHome.GrpcService;
public class GrpcClient
{
    GrpcChannel GrpcChannel;// 
    public GrpcClient()
    {
        // 自动连接好吗 这不好
        GrpcChannel = GrpcChannel.ForAddress($"http://172.168.35.77:45152");
        GrpcChannel.ConnectAsync().Wait();
        // 如果不成功 需要提示这类
    }
    // 先可考虑唯一客户端
    public Greeter.GreeterClient GetInstance()
    {
        return new Greeter.GreeterClient(GrpcChannel);
    }
}
