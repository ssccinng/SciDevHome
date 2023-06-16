using MediatR;

namespace SciDevHome.Server.Mediator.Queries;


/// <summary>
/// 获取所有客户端
/// </summary>
public record GetClientQuery: IRequest<IEnumerable<ClientInfo>>;