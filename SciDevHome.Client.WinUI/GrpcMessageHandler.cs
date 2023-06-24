﻿using Grpc.Core;
using SciDevHome.Message;
using SciDevHome.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SciDevHome.API;
using SciDevHome.Providers;

namespace SciDevHome.Utils
{
    public static class GrpcMessageHandler
    {
        public static async Task ServerMessageHander(
            IClientStreamWriter<ConnectRequest> requestStream,
            ConnectRequest response)
        {
            // ?? 这个data是不是不太对
            var resdata = await ConnectAPI.SendReplyAsync(
                ConnectProvider.GetProvider(response.Cmd), 
                response);
            // 先不分段？？
            await requestStream.WriteAsync(resdata);
        
        }
    }
}