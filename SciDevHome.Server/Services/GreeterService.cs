using Grpc.Core;
using MediatR;
using SciDevHome.Message;
using SciDevHome.Server;
using SciDevHome.Server.Model;
using System.Collections.Concurrent;
using System.Text.Json;

namespace SciDevHome.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly DevHomeDb _devHomeDb;

        // 可能亲求也需要标记
        private static ConcurrentDictionary<string, IServerStreamWriter<ConnectResponse>> _keyValuePairs = new();

        public GreeterService(ILogger<GreeterService> logger, DevHomeDb devHomeDb)
        {
            _logger = logger;
            _devHomeDb = devHomeDb;
        }
        private string GetClientIpAddress(ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            if (headers != null && headers.Any(x => x.Key == "x-forwarded-for"))
            {
                // 如果使用了代理服务器，则通过 x-forwarded-for 头部获取客户端的 IP
                return headers.Where(x => x.Key == "x-forwarded-for").FirstOrDefault()?.Value?.ToString().Split(',')[0]?.Trim();
            }
            else
            {
                // 如果没有使用代理服务器，则直接获取 RemoteIpAddress 属性的值
                return context.Peer;
            }
        }

        public override Task<RegisterResponse> Register(ClientInfo request, ServerCallContext context)
        {
            var user = new User
            {
                ClientId = Random.Shared.Next(100_000_000_0).ToString(),
                UserName = "Test User",
                Password = "password",
                Mac = request.Mac,
            };

            _devHomeDb.Users.Add(user);
            _devHomeDb.SaveChanges();

            return Task.FromResult(new RegisterResponse
            {
                 ClientId = user.ClientId,
            });
        }

        public override Task<DevMessage> SendMessage(DevMessage request, ServerCallContext context)
        {
            Console.WriteLine(request.Json);

            var aa = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Json);
            foreach (var item in aa)
            {
                Console.WriteLine(  item.Key);
                Console.WriteLine(  item.Value.GetRawText());
            }

            return Task.FromResult(new DevMessage
            {
                Type = "临流揽镜夜双魂",
                Json = request.Json,
            }); 
        }

        public override Task<GetPathResponse> GetClientPath(GetPathRequest request, ServerCallContext context)
        {
            _keyValuePairs[request.ClientId].WriteAsync(new ConnectResponse
            {
                Cmd = "getPathInfo",
                Data = JsonSerializer.Serialize(new GetPathRequestMessage
                {
                    Path = request.Path,
                })
            });

            return Task.FromResult(new GetPathResponse { Path = request.Path });
        }

        public override async Task Connect(IAsyncStreamReader<ConnectRequest> requestStream, IServerStreamWriter<ConnectResponse> responseStream, ServerCallContext context)
        {
            // 管理所有连接，当连接断开时，从列表中移除
            // 通过 context.CancellationToken.IsCancellationRequested 判断连接是否断开

            Guid id = Guid.NewGuid();
            if (!_keyValuePairs.ContainsKey(" "))
            {
                _keyValuePairs.TryAdd(" ", responseStream);
            }
            _logger.LogInformation($"New connection: {id}");
            try
            {
                _logger.LogInformation($"Message from {GetClientIpAddress(context)}");
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // 这里很可能需要修改
                    if (requestStream.MoveNext().Result)
                    {
                        var request = requestStream.Current;
                        GrpcMessageHandler.ClientConnectMessageHandler(responseStream, request);
                        // 等待接受一条初始化信息
                        //await responseStream.WriteAsync(new ConnectResponse { Message = $"Hello {request.Info}" });
                    }
                    //await Task.Delay(1000, context.CancellationToken);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation($"connection: {id} disconnect {ex.Message}");
            }
            

        }
    }
}