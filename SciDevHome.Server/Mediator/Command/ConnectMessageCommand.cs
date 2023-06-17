using MediatR;

namespace SciDevHome.Server.Mediator.Command;

public record ConnectMessageCommand(ConnectRequest request, string connectId) : IRequest;