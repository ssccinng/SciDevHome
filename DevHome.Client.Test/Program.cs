using Grpc.Core;
using Grpc.Net.Client;
using SciDevHome;
using System.Formats.Asn1;
using System.Text.Json;
using System.Threading.Channels;

namespace DevHome.Client.Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var saves = await SaveFileManager.LoadAsync("devhomeSetting.json");
            using var channel = GrpcChannel.ForAddress($"http://{saves.IdServer}:{SciDevHomeCommon.DefaultPort}");

            await channel.ConnectAsync();

            var client = new SciDevHome.Server.Greeter.GreeterClient(channel);


            if (saves.ClientId == string.Empty)
            {
                var regRes = await client.RegisterAsync(new SciDevHome.Server.ClientInfo { Ip = "dabu", Name = "aa" });
                saves.ClientId = regRes.ClientId;
                await SaveFileManager.SaveAsync("devhomeSetting.json", saves);
            }

            // 检测本地有没有
            var stream =  client.Connect();
            new Thread(() => { ListenServer(stream); }).Start();
            await client.SendMessageAsync(
                new SciDevHome.Server.DevMessage { 
                    
                    Type = "FolderPath",
                    Json = JsonSerializer.Serialize(
                        new
                        {
                            Message = Directory.GetDirectories("D:")
                        }, new JsonSerializerOptions
                        {
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping

                        }
                    ) }
                );
            while (true)
            {
                var path =
            Console.ReadLine();
                await client.GetClientPathAsync(new SciDevHome.Server.GetPathRequest { ClientId = " ", Path = path });
            }
        }

        public static async void ListenServer(Grpc.Core.AsyncDuplexStreamingCall<SciDevHome.Server.ConnectRequest, SciDevHome.Server.ConnectResponse> stream)
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
}