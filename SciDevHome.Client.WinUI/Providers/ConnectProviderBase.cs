using System.Text.Json;
using Grpc.Net.ClientFactory;
using SciDevHome.Client.WinUI;
using SciDevHome.Message;
using SciDevHome.Server;
using SciDevHome.Utils;

namespace SciDevHome.Providers;

// 连接信息功能提供者
public class ConnectProvider
{
    private static Dictionary<string, ConnectProvider>
        _directory = new();

    public static ConnectProvider GetPathInfoProvider
        = new ConnectProvider("GetPathInfo",
            data =>
            {
                try
                {
                    List<GrpcDirctoryInfo> list = new();
                    var getpm =
                        JsonSerializer.Deserialize<GetPathRequestMessage>(
                            data);
                    if (getpm.Path == "")
                    {
                        // 获取磁盘
                        var rootFolders = ZQDHelper.GetRootPath();
                        foreach (var file in rootFolders)
                        {
                            list.Add(new GrpcDirctoryInfo()
                                { Path = file.Name, IsDirectory = true });
                        }

                        return list;
                    }

                    var directory = new DirectoryInfo(getpm.Path);
                    var files = directory.GetFiles();
                    var dirs = directory.GetDirectories();
                    foreach (var dir in dirs)
                    {
                        list.Add(new GrpcDirctoryInfo()
                        {
                            LastWriteTime = dir.LastWriteTime,
                            Path = dir.FullName, IsDirectory = true
                        });
                    }

                    foreach (var file in files)
                    {
                        list.Add(new GrpcDirctoryInfo()
                        {
                            Path = file.FullName, IsDirectory = false,
                            LastWriteTime = file.LastWriteTime
                        });
                    }

                    return list;
                }
                catch
                {
                    return null;
                }

                return new object();
            }
        );


    public static ConnectProvider InitClientProvider = new ConnectProvider(
        "InitClient",
        data =>
        {
            throw new Exception();
        });

// 对于download 属于远程更新的情况
    public static ConnectProvider DownloadFileProvider = new ConnectProvider(
        "DownloadFile",
        data =>
        {
            // 额？给我去访问接口 哼哼 啊啊
            throw new NotImplementedException();
        });

    public static ConnectProvider UploadFileProvider = new ConnectProvider(
        "UploadFile",
        s =>
        {
            var client = App.GetService<GrpcClientFactory>().CreateClient<Greeter.GreeterClient>("test");
            var aa = client.UploadFile();
            var bb =  File.ReadAllBytes(s);
            bb.Chunk(4096);
            // 上传所有
            // 去上传！！
            return "ok";
        });

    public string Command
    {
        get;
    }

    public ConnectProvider(string command,
        Func<string, object> handler)
    {
        Command = command;
        Handler = handler;
        // 初始化可能存在问题
        _directory.TryAdd(command, this);
    }
    // 要不所有都回复，区别只是收不收
    // public ConnectResponse Handler();

    public Func<string, object> Handler;

    public static ConnectProvider GetProvider(string cmd)
    {
        if (_directory.ContainsKey(cmd))
        {
            return _directory[cmd];
        }

        throw new Exception("没有此命令");
    }
}

// 这里统一进行cmd与provider转换？

// /// <summary>
// /// 获取路径提供者
// /// </summary>
// public class GetPathInfoProvider : ConnectProviderBase
// {
//     
// }