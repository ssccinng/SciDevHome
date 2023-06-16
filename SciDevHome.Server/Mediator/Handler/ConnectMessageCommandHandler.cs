using MediatR;
using SciDevHome.Server.Mediator.Command;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;

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

        return Task.CompletedTask;
    }
}
