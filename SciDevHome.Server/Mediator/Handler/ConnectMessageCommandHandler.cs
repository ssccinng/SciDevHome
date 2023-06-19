using System.Text.Json;
using MediatR;
using SciDevHome.Message;
using SciDevHome.Server.Mediator.Command;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;
/// <summary>
/// 连接信息
/// </summary>
public class ConnectMessageCommandHandler : IRequestHandler<ConnectMessageCommand>
{
    private readonly DevHomeService _devHomeService;
    private readonly IMediator _mediator;

    public ConnectMessageCommandHandler(DevHomeService devHomeService, IMediator mediator)
    {
        _devHomeService = devHomeService;
        _mediator = mediator;
    }
    public Task Handle(ConnectMessageCommand request, CancellationToken cancellationToken)
    {
        
        switch (request.request.Cmd)
        {
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
