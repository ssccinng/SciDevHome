﻿using Grpc.Core;
using System.Collections.Concurrent;
using MediatR;
using Microsoft.VisualBasic;
using SciDevHome.Server.Data;

namespace SciDevHome.Server.Services;

public class DevHomeService
{
    private readonly IMediator _mediator;
    private readonly DevHomeDB _devHomeDB;
    private readonly ILogger<DevHomeDB> _logger;

    /// <summary>
    /// 客户端对应的流表，是否封装一下会更好
    /// </summary>
    public ConcurrentDictionary<string, ClientConnectInfo> ClientDict = new();
    public DevHomeService(IMediator mediator, ILogger<DevHomeDB> logger)
    {
        _mediator = mediator;
        //_devHomeDB = App;
        _logger = logger;
    }

    // 加入一个连接
    public void AddConnect(ClientConnectInfo clientConnectInfo)
    {
        // Todo: 判断一下, 不允许已经有的再次进入
        if (ClientDict.ContainsKey(clientConnectInfo.ConnectId))
        {
            _logger.LogWarning("{ID} 已注册", clientConnectInfo.ConnectId);
            return;
        }
        ClientDict.TryAdd(clientConnectInfo.ConnectId, clientConnectInfo);
    }

    internal void RemoveConnect(ClientConnectInfo clientConnectInfo)
    {
        // 删除id
        if(!ClientDict.ContainsKey(clientConnectInfo.ConnectId))
        {
            _logger.LogWarning("{ID} 未注册", clientConnectInfo.ConnectId);
            return;
        }
        // 尝试删除
        ClientDict.TryRemove(clientConnectInfo.ConnectId, out _);
        //ClientDict.TryRemove();
    }

    internal void UpdateClientId(string connectId, string clientId, string name)
    {
        if (ClientDict.TryGetValue(connectId, out var clientConnectInfo))
        {
            clientConnectInfo.ClientId = clientId;
            clientConnectInfo.Name = name;
        }
        else 
        {
            _logger.LogError($"UpdateClientId: 未找到connectId: {connectId}");
        }
    }
}


/// <summary>
/// 客户端连接信息 // 并撕烤需不需要持久化
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
    public required IServerStreamWriter<ConnectRequest> ServerStreamWriter
    {
        get; set;
    }
    public string Name
    {
        get;
        set;
    } = string.Empty;

    //public


}