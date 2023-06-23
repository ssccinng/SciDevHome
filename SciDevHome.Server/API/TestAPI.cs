using System.Text.Json;
using SciDevHome.Providers;

namespace SciDevHome.Server.API;

public class TestAPI
{
    public static async Task<ConnectRequest> SendRequestAsync(
        ConnectProvider provide, object Data)
    {
        return new ConnectRequest
        {
            Cmd = provide.Command,
            Data = JsonSerializer.Serialize(Data),
            ReqId = Guid.NewGuid().ToString(),
        };
    }
}