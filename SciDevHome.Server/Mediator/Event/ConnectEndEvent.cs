using MediatR;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Event;

public record ConnectEndEvent(ClientConnectInfo ClientConnectInfo) : IRequest;
