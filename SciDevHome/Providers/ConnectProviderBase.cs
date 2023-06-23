using System.Text.Json;
using SciDevHome.Message;
using SciDevHome.Server;
using SciDevHome.Utils;

namespace SciDevHome.Providers;


// 连接信息功能提供者
public class ConnectProvider
{
    private static Dictionary<string, ConnectProvider>
        _directory = new ();

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

                    

                }
                catch
                {
                    throw;
                }
                return new object();
            }
        );
    public string Command
    {
        get;
    }

    public ConnectProvider(string command, 
        Func<string, object> handler)
    {
        Command = command;
        Handler = handler;
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