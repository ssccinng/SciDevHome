using Grpc.Core;
using System.Collections.Concurrent;
using MediatR;

namespace SciDevHome.Server.Services;

public class DevHomeService
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 客户端对应的流表，是否封装一下会更好
    /// </summary>
    public ConcurrentDictionary<string, IServerStreamWriter<ConnectResponse>> ClientDict = new();
    public DevHomeService(IMediator mediator)
    {
        _mediator = mediator;
    }


    public void AddConnect()
    {
    
    }


}
