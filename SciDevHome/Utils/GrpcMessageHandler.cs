using Grpc.Core;
using SciDevHome.Message;
using SciDevHome.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SciDevHome.Utils
{
    public static class GrpcMessageHandler
    {
        public static async Task ServerMessageHander(IClientStreamWriter<ConnectRequest> requestStream, ConnectResponse response)
        {
            switch (response.Cmd)
            {
                case "getPathInfo":

                    List<GrpcDirctoryInfo> list = new();

                    // 标准化maybe
                    // yes need 模块化
                    var getpm = JsonSerializer.Deserialize<GetPathRequestMessage>(response.Data);
                    if (getpm.Path == "")
                    {
                        var rootFolders = ZQDHelper.GetRootPath();
                        foreach (var file in rootFolders)
                        {
                            list.Add(new GrpcDirctoryInfo() { Path = file.Name, IsDirectory = true });
                        }
                        // 返回
                        await requestStream.WriteAsync(new ConnectRequest { Cmd = "pathInfo", Data = JsonSerializer.Serialize(list), ReqId = response.ReqId });
                        return; // 要
                                // 返回zqd
                    }
                    
                    // 为空判断
                    var directory = new DirectoryInfo(getpm.Path);


                    var files = directory.GetFiles();
                    var dirs = directory.GetDirectories();

                    foreach (var file in files)
                    {
                        list.Add(new GrpcDirctoryInfo() { Path = file.FullName, IsDirectory = false, LastWriteTime = file.LastWriteTime });
                    }

                    foreach (var dir in dirs)
                    {
                        list.Add(new GrpcDirctoryInfo() { LastWriteTime = dir.LastWriteTime, Path = dir.FullName, IsDirectory = true });
                    }
                    
                    // Todo: 都得返回这个reqid 最好能有截面统一管理
                    // 思考类型的管理
                    await requestStream.WriteAsync(new ConnectRequest { Cmd = "pathInfo", Data = JsonSerializer.Serialize(list), ReqId = response.ReqId});
                    break;
                default:
                    break;
            }

        }

    }
}
