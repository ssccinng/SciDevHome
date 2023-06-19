using MediatR;
using SciDevHome.Server.Mediator.Queries;
using SciDevHome.Server.Services;

namespace SciDevHome.Server.Mediator.Handler;

/// <summary>
/// 获取所有的客户端
/// </summary>

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, IEnumerable<ClientInfo>>
{
    private readonly DevHomeService _devHomeService;

    public GetClientQueryHandler(DevHomeService devHomeService)
    {
        _devHomeService = devHomeService;
    }
    public Task<IEnumerable<ClientInfo>> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_devHomeService.ClientDict.Select(s => new ClientInfo { ClientId = s.Value.ClientId }));
    }
}
