using Grpc.Core;
using System.Collections.Concurrent;
using MediatR;
using Microsoft.VisualBasic;
using SciDevHome.Server.Data;

namespace SciDevHome.Server.Services;

public class DevHomeService
{
    private readonly IMediator _mediator;
    private readonly DevHomeDB _devHomeDB;

    /// <summary>
    /// 客户端对应的流表，是否封装一下会更好
    /// </summary>
    public ConcurrentDictionary<string, ClientConnectInfo> ClientDict = new();
    public DevHomeService(IMediator mediator, DevHomeDB devHomeDB, ILogger<DevHomeDB> logger)
    {
        _mediator = mediator;
        _devHomeDB = devHomeDB;
    }


    public void AddConnect(ClientConnectInfo clientConnectInfo)
    {
        // Todo: 判断一下, 不允许已经有的再次进入
        ClientDict.TryAdd(clientConnectInfo.ConnectId, clientConnectInfo);
    }

    internal void UpdateClientId(string connectId)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// 客户端连接信息
/// </summary>
public class ClientConnectInfo
{
    /// <summary>
    /// 客户端id
    /// </summary>
    public string ClientId
    {
        get; set;
    } = string.Empty;

    /// <summary>
    /// 本次连接id
    /// </summary>
    public string ConnectId
    {
        get; set;
    } = string.Empty;
    /// <summary>
    /// 若结束则此成员不可访问
    /// </summary>
    public required IServerStreamWriter<ConnectResponse> ServerStreamWriter
    {
        get; set;
    }

    //public


}