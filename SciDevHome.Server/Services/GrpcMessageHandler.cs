using SciDevHome.Message;
using System.Text.Json;

namespace SciDevHome.Server.Services
{
    public static class GrpcMessageHandler
    {
        public static void ClientConnectMessageHandler(Grpc.Core.IServerStreamWriter<ConnectResponse> responseStream, ConnectRequest request)
        {
            //Console.WriteLine(JsonSerializer.Serialize(request.Data, new JsonSerializerOptions
            //{
            //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //     WriteIndented = true,
            //}));
            switch (request.Cmd)
            {
                case "init":
                    // 初始化
                    break;
                case "pathInfo":
                    Console.WriteLine("pathInfo");
                    var daa = JsonSerializer.Deserialize<GrpcDirctoryInfo[]>(request.Data);
                    foreach (var item in daa)
                    {
                        Console.WriteLine(item.Path);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    
    public class ConnectMessage
    {
        public string Cmd { get; set; }
        public JsonElement Data { get; set; }
    }
}
