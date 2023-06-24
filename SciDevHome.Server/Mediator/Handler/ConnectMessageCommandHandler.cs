using System.Text.Json;
using MediatR;
using SciDevHome.Message;
using SciDevHome.Server.Mediator.Command;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;
/// <summary>
/// 处理grpc连接信息
/// </summary>
public class ConnectMessageCommandHandler : IRequestHandler<ConnectMessageCommand>
{
    private readonly DevHomeService _devHomeService;
    private readonly IMediator _mediator;
    private readonly StreamGrpcManager _streamGrpcManager;

    public ConnectMessageCommandHandler(DevHomeService devHomeService, IMediator mediator, StreamGrpcManager streamGrpcManager)
    {
        _devHomeService = devHomeService;
        _mediator = mediator;
        _streamGrpcManager = streamGrpcManager;
    }
    public Task Handle(ConnectMessageCommand request, CancellationToken cancellationToken)
    {
        if (request.request.ReqId != String.Empty)
        {
            _streamGrpcManager.AddResponse(request.request);
        }
        
        // 这里..
        switch (request.request.Cmd)
        {
            // 是否已经取消？？
            case "InitClient":
                // 序列化
                var initClientData = JsonSerializer.Deserialize<ClientIdUpdateMessage>(request.request.Data);
                _devHomeService.UpdateClientId(request.connectId, initClientData.ClientId, initClientData.Name);
                break;
            case "GetPathInfo":
                
                break;
            case "":
                break;
            default:
                break;
        }
        return Task.CompletedTask;
    }
}
