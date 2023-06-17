using MediatR;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Event;

public record ConnectStartEvent(ClientConnectInfo ClientConnectInfo) : IRequest;
