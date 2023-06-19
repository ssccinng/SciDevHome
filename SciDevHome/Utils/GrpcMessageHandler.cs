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
                    // 标准化maybe
                    // yes need 模块化
                    var getpm = JsonSerializer.Deserialize<GetPathRequestMessage>(response.Data);
                    // 为空判断
                    var directory = new DirectoryInfo(getpm.Path);


                    var files = directory.GetFiles();
                    var dirs = directory.GetDirectories();
                    List<GrpcDirctoryInfo> list = new();

                    foreach (var file in files)
                    {
                        list.Add(new GrpcDirctoryInfo() { Path = file.FullName, IsDirectory = false, LastWriteTime = file.LastWriteTime });
                    }

                    foreach (var dir in dirs)
                    {
                        list.Add(new GrpcDirctoryInfo() { LastWriteTime = dir.LastWriteTime, Path = dir.FullName, IsDirectory = true });
                    }

                    await requestStream.WriteAsync(new ConnectRequest { Cmd = "pathInfo", Data = JsonSerializer.Serialize(list) });
                    break;
                default:
                    break;
            }

        }

    }
}
