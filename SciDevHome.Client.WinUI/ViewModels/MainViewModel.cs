using CommunityToolkit.Mvvm.ComponentModel;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using SciDevHome.Server;
using SciDevHome.Utils;
using Windows.Media.Protection.PlayReady;

namespace SciDevHome.Client.WinUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly GrpcClientFactory _grpcClientFactory;
    public readonly Greeter.GreeterClient Client;
    public static DevHomeClientSave Saves;

    public MainViewModel(GrpcClientFactory grpcClientFactory)
    {
        _grpcClientFactory = grpcClientFactory;
        Client = grpcClientFactory.CreateClient<Greeter.GreeterClient>("test");
        // 思考用在其他地方

        //Init().Wait();
    }

   

    public static async void ListenServer(AsyncDuplexStreamingCall<ConnectRequest, ConnectRequest> stream)
    {
        while (true)
        {
            try
            {
                if (stream.ResponseStream.MoveNext().Result)
                {
                    var response = stream.ResponseStream.Current;

                    await GrpcMessageHandler.ServerMessageHander(stream.RequestStream, response);
                    //new getpa
                    // 等待接受一条初始化信息
                    //await responseStream.WriteAsync(new ConnectResponse { Message = $"Hello {request.Info}" });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
