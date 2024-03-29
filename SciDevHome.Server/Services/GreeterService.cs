using Grpc.Core;
using MediatR;
using SciDevHome.Message;
using SciDevHome.Server;
using SciDevHome.Server.Mediator.Command;
using SciDevHome.Server.Mediator.Event;
using SciDevHome.Server.Mediator.Queries;
using SciDevHome.Server.Model;
using System.Collections.Concurrent;
using System.Text.Json;
using Google.Protobuf;
using SciDevHome.Server.API;

namespace SciDevHome.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly DevHomeDb _devHomeDb;
        private readonly IMediator _mediator;
        private readonly StreamGrpcManager _streamGrpcManager;

        // 可能亲求也需要标记

        public GreeterService(ILogger<GreeterService> logger,
            DevHomeDb devHomeDb, IMediator mediator,
            StreamGrpcManager streamGrpcManager)
        {
            _logger = logger;
            _devHomeDb = devHomeDb;
            _mediator = mediator;
            _streamGrpcManager = streamGrpcManager;
        }

        private string GetClientIpAddress(ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            if (headers != null && headers.Any(x => x.Key == "x-forwarded-for"))
            {
                // 如果使用了代理服务器，则通过 x-forwarded-for 头部获取客户端的 IP
                return headers.Where(x => x.Key == "x-forwarded-for")
                    .FirstOrDefault()?.Value?.ToString().Split(',')[0]?.Trim();
            }
            else
            {
                // 如果没有使用代理服务器，则直接获取 RemoteIpAddress 属性的值
                return context.Peer;
            }
        }


        public override Task<RegisterResponse> Register(ClientInfo request,
            ServerCallContext context)
        {
            // 查证一下
            // 也许需要指令化
            var user = new User
            {
                // 这个地方 要查验数据库确保两者id不同！或是直接guid?
                ClientId = Random.Shared.Next(100_000_000_0).ToString(),
                UserName = "Test User",
                Password = "password",
                Mac = request.Mac,
            };

            _devHomeDb.Users.Add(user);
            _devHomeDb.SaveChanges();

            return Task.FromResult(new RegisterResponse
            {
                ClientId = user.ClientId,
            });
        }

        public override Task<DevMessage> SendMessage(DevMessage request,
            ServerCallContext context)
        {
            Console.WriteLine(request.Json);

            var aa =
                JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                    request.Json);
            foreach (var item in aa)
            {
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value.GetRawText());
            }

            return Task.FromResult(new DevMessage
            {
                Type = "临流揽镜夜双魂",
                Json = request.Json,
            });
        }

        public override async Task<GetPathResponse> GetClientPath(
            GetPathRequest request, ServerCallContext context)
        {
            // 需要有管理机制，等待客户端回复，或超时等
            // 这都没send出去，都只能算构造了
            
            var reqData =
                await TestAPI.SendRequestAsync(
                    //ConnectProvider.GetPathInfoProvider,
                    "GetPathInfo",
                    new GetPathRequestMessage
                    {
                        Path = request.Path,
                    }); // 更加那个啥needwait创建（？
            var res = await _streamGrpcManager.SendConnectStreamAsync(
                request.ClientId, reqData);
            var getp = new GetPathResponse { Path = request.Path, IsSucc = true};

            if (res.Data == "null")
            {
                getp.IsSucc = false;
                // 无权限什么的消息，也许需要返回
                return getp;
            }
            // todo: 转换是不是太多了（?
            var gg =
                JsonSerializer.Deserialize<List<GrpcDirctoryInfo>>(res.Data); // 如果是null 则需返回失败
            getp.Files.AddRange(gg.Select(s => new GrpcFileInfo
                { IsDirectory = s.IsDirectory, Name = s.Path }));
            return getp;
        }

        /// <summary>
        /// 获取所有在线客户端， // 客户端本地也需缓存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetClientsResponse> GetClients(
            GetClientsRequest request, ServerCallContext context)
        {
            var res = new GetClientsResponse();
            var clients = await _mediator.Send(new GetClientQuery());
            ;
            res.Clients.AddRange(clients);

            return res;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Connect(
            IAsyncStreamReader<ConnectRequest> requestStream,
            IServerStreamWriter<ConnectRequest> responseStream,
            ServerCallContext context)
        {
            // 管理所有连接，当连接断开时，从列表中移除 ConnectInitCommand
            // 通过 context.CancellationToken.IsCancellationRequested 判断连接是否断开

            // 连接id

            //Metadata headers = context.req

            // 生成id
            var connectionId = Guid.NewGuid().ToString();

            var cinfo = new ClientConnectInfo
            {
                ServerStreamWriter = responseStream,
                ConnectId = connectionId
            };

            // 初始化本次连接
            await _mediator.Send(new ConnectStartEvent(cinfo));
            // Todo: 还需要重连代码desuwa


            _logger.LogInformation($"New connection: {connectionId}");
            try
            {
                _logger.LogInformation(
                    $"Message from {GetClientIpAddress(context)}");
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // 这里很可能需要修改
                    if (requestStream.MoveNext().Result)
                    {
                        var request = requestStream.Current;
                        // 发起连接信息 中介者处理连接信息 让中介者处理这个？？
                        await _mediator.Send(
                            new ConnectMessageCommand(request, connectionId));

                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(
                    $"connection: {connectionId} disconnect {ex.Message}");
                // 移除此连接
            }
            finally
            {
                await _mediator.Send(new ConnectEndEvent(cinfo));
            }
        }

        /// <summary>
        /// 从服务端下载文件
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task DownloadFile(DownloadFileRequest request,
            IServerStreamWriter<DownloadFileResponse> responseStream,
            ServerCallContext context)
        {
            // 要check客户端的文件 比如md5这类
            // await _streamGrpcManager.SendConnectStreamAsync("");
            // 1. 从通知指定客户端上传文件
            
            var reqData =
                await TestAPI.SendRequestAsync(
                    "UploadFile",
                    new GetPathRequestMessage
                    {
                        Path = request.Path,
                    }); // 更加那个啥needwait创建（？
            var res = await _streamGrpcManager.SendConnectStreamAsync(request.ClientId, reqData);
            // 2. 等待客户端文件上传
            // 3. 再把获取后的文件返回给客户端
            // 获取的这个好吗？
            // todo: 需要剧烈思考
            var bb = File.ReadAllBytes(JsonSerializer.Deserialize<GetPathResponseMessage>(res.Data).Path);
            foreach (var datas in bb.Chunk(4096))
            {
                var dd = new DownloadFileResponse
                {
                    Data = ByteString.CopyFrom(datas)
                };
                await responseStream.WriteAsync(dd);
            }
            
            // responseStream
        }

        /// <summary>
        /// 客户端上传文件至服务端
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UploadFileResponse> UploadFile(
            IAsyncStreamReader<UploadFileRequest> requestStream,
            ServerCallContext context)
        {
            
            // if (Directory.Exists($"Temp/{requestStream}"))
            
            
             // 1. 创建一个临时文件，用于存储客户端上传的文件数据
            var tempFilePath = Path.GetTempFileName();
            using var fileStream = File.Create(tempFilePath);

            // 2. 从客户端流中读取文件数据，并将其写入临时文件中
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await fileStream.WriteAsync(request.Data.ToByteArray());
            }
            fileStream.Close();
            // File.Move(tempFilePath, );
            // 3. 返回一个响应消息，表示文件上传成功
            return new UploadFileResponse
            {
                Path = tempFilePath
            };
        }
    }
}