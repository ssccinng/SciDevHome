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
            await client.RegisterAsync(new SciDevHome.Server.ClientInfo { Ip = "dabu", Name = "aa" });

            var stream =  client.Connect();

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
            Console.ReadLine();
        }
    }


    class SaveFileManager
    {
        public static async Task SaveAsync(string fileName, DevHomeClientSave devHomeClientSave)
        {
            await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(devHomeClientSave));
        }

        public static async Task<DevHomeClientSave> LoadAsync(string fileName)
        {
            try
            {
                return JsonSerializer.Deserialize<DevHomeClientSave>(File.ReadAllText(fileName));

            }
            catch (Exception)
            {

                var res = new DevHomeClientSave();
                await SaveAsync(fileName, res);
                return res;
            }
        }

    }
}