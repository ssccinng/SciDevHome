using MediatR;
using SciDevHome.Server.Mediator.Command;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;
[Obsolete]
public class ConnectInitCommandHandler : IRequestHandler<ConnectInitCommand>
{
    private readonly DevHomeService _devHomeService;

    public ConnectInitCommandHandler(DevHomeService devHomeService)
    {
        _devHomeService = devHomeService;
    }
    public Task Handle(ConnectInitCommand request, CancellationToken cancellationToken)
    {
        //_devHomeService.AddConnect(request);
        return Task.CompletedTask;
    }
}
