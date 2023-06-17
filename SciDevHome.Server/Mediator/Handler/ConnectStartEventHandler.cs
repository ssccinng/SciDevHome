﻿using MediatR;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;

public class ConnectStartEventHandler : IRequestHandler<ConnectStartEvent>
{
    private readonly IMediator _mediator;
    private readonly DevHomeService _devHomeService;

    public ConnectStartEventHandler(IMediator mediator, DevHomeService devHomeService)
    {
        _mediator = mediator;
        _devHomeService = devHomeService;
    }
    public Task Handle(ConnectStartEvent request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
