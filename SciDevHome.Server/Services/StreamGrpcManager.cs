using System.Collections.Concurrent;
using System.Text.Json;
using Grpc.Net.ClientFactory;

namespace SciDevHome.Server.Services;

public class StreamGrpcManager
{
    private readonly DevHomeService _devHomeService;
    // private readonly GrpcClientFactory _grpcClientFactory;
    //
    // public StreamGrpcManager(GrpcClientFactory grpcClientFactory)
    // {
    //     _grpcClientFactory = grpcClientFactory;
    // }

    // 等待
    private ConcurrentDictionary<string, ConnectRequest?> _messages = new();

    public StreamGrpcManager(DevHomeService devHomeService)
    {
        _devHomeService = devHomeService;
    }
    
    // 是佛要更具体的类型
    public void AddResponse(ConnectRequest connectRequest)
    {
        // todo: 需要安全化吗
        _messages.TryUpdate(connectRequest.ReqId, connectRequest, null);
    } 
    public async Task<ConnectRequest> SendConnectStreamAsync(string clientId, ConnectResponse connectResponse)
    {
        var connectResponseReqId = connectResponse.ReqId;
        //
        // // 也有我不想要回复的
        // connectResponse.ReqId = connectResponseReqId;
        // 一百秒超时（？ 可能要根据信息种类设定
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(100));
        if (connectResponse.ReqId != string.Empty)
        {
            _messages.TryAdd(connectResponse.ReqId, null);

        }
        else
        {
            // 不需要
            return new ConnectRequest();
        }
        // 插入字典
        
        
        var client = _devHomeService.ClientDict.FirstOrDefault(s => s.Value.ClientId == clientId);
        if (client.Value != null)
        {
            await client.Value.ServerStreamWriter.WriteAsync(connectResponse);

            try
            {
                // 有一丢丢费性能 考虑
                while (_messages[connectResponseReqId] == null)
                {
                    await Task.Delay(10, cancellationTokenSource.Token);
                }
                // 返回值
                var res = _messages[connectResponseReqId]!;
                _messages.TryRemove(connectResponseReqId, out _);
                return res;
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
                return new ConnectRequest {Cmd = "Error", Data = "超时未响应" };
            }
        }
        else
        {
            // Todo: 返回不存在 connecnt新增类型
        }
        
        throw new NotImplementedException();
    }
}