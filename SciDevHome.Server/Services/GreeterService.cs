using Grpc.Core;
using SciDevHome.Server;

namespace SciDevHome.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
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
        public override async Task Connect(IAsyncStreamReader<ConnectRequest> requestStream, IServerStreamWriter<ConnectResponse> responseStream, ServerCallContext context)
        {
            // 管理所有连接，当连接断开时，从列表中移除
            // 通过 context.CancellationToken.IsCancellationRequested 判断连接是否断开

            Guid id = Guid.NewGuid();
            _logger.LogInformation($"New connection: {id}");
            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {

                    // 这里很可能需要修改
                    if (requestStream.MoveNext().Result)
                    {
                        var request = requestStream.Current;
                        _logger.LogInformation($"Message from {id}: {request.Info.IP}");
                        await responseStream.WriteAsync(new ConnectResponse { Message = $"Hello {request.Info}" });
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation($"connection: {id} disconnect {ex.Message}");
            }
            

        }
    }
}