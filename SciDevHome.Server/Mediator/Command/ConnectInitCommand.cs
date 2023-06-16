using MediatR;

namespace SciDevHome.Server.Mediator.Command;


/// <summary>
/// 连接初始化(返回当前连接(? ) 考虑入参为ClientInfo
/// </summary>
public record ConnectInitCommand(Guid Guid, string ClientId): IRequest;

