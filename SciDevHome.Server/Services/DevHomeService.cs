using Grpc.Core;
using System.Collections.Concurrent;
using MediatR;
using Microsoft.VisualBasic;

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


    public void AddConnect(ClientConnectInfo clientConnectInfo)
    {

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

    public required IServerStreamWriter<ConnectResponse> ServerStreamWriter
    {
        get; set;
    }

    //public


}