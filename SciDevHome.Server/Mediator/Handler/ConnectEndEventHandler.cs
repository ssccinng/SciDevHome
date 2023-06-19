using MediatR;
using SciDevHome.Server.Mediator.Event;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;

public class ConnectEndEventHandler : IRequestHandler<ConnectEndEvent>
{
    private readonly DevHomeService _devHomeService;

    public ConnectEndEventHandler(IMediator mediator, DevHomeService devHomeService)
    {
        _devHomeService = devHomeService;
    }
    public Task Handle(ConnectEndEvent request, CancellationToken cancellationToken)
    {
        _devHomeService.RemoveConnect(request.ClientConnectInfo);
        return Task.CompletedTask;
    }
}
