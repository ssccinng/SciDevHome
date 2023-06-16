using MediatR;

namespace SciDevHome.Server.Mediator.Command;

public record ConnectMessageCommand(ConnectRequest request, Grpc.Core.IServerStreamWriter<ConnectResponse> responseStream) : IRequest;