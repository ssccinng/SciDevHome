using MediatR;
using SciDevHome.Data;

namespace SciDevHome.Server.Mediator.Command
{
    /// <summary>
    /// 注册指令
    /// </summary>
    /// <param name=""></param>
    public record RegisterCommand(string clientId, string IP) : IRequest<HomeUser>;
}
