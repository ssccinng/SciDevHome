using MediatR;

namespace SciDevHome.Server.Mediator.Command
{
    /// <summary>
    /// 根据id，group更新文件，并且
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="Group"></param>
    public record UpdateFileCommand(string ClientId, string Group): IRequest<string>;
}
