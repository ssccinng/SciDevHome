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

namespace SciDevHome.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly DevHomeDb _devHomeDb;
        private readonly IMediator _mediator;
        private readonly StreamGrpcManager _streamGrpcManager;

        // 可能亲求也需要标记

        public GreeterService(ILogger<GreeterService> logger, DevHomeDb devHomeDb, IMediator mediator, StreamGrpcManager streamGrpcManager)
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
                return headers.Where(x => x.Key == "x-forwarded-for").FirstOrDefault()?.Value?.ToString().Split(',')[0]?.Trim();
            }
            else
            {
                // 如果没有使用代理服务器，则直接获取 RemoteIpAddress 属性的值
                return context.Peer;
            }
        }


        public override Task<RegisterResponse> Register(ClientInfo request, ServerCallContext context)
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

        public override Task<DevMessage> SendMessage(DevMessage request, ServerCallContext context)
        {
            Console.WriteLine(request.Json);

            var aa = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Json);
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

        public override async Task<GetPathResponse> GetClientPath(GetPathRequest request, ServerCallContext context)
        {
            
            // 需要有管理机制，等待客户端回复，或超时等
            var res = await _streamGrpcManager.SendConnectStreamAsync(request.ClientId, new ConnectResponse
            {
                Cmd = "getPathInfo",
                Data = JsonSerializer.Serialize(new GetPathRequestMessage
                {
                    Path = request.Path,
                }),
                ReqId = Guid.NewGuid().ToString()
                // 也许可优化
                
                
            });
            // todo: 转换是不是太多了（?
            var gg = JsonSerializer.Deserialize<List<GrpcDirctoryInfo>>(res.Data);
            var getp = new GetPathResponse { Path = request.Path };
            getp.Files.AddRange(gg.Select(s => new GrpcFileInfo { IsDirectory = s.IsDirectory, Name = s.Path}));
            return getp;
        }
        /// <summary>
        /// 获取所有在线客户端， // 客户端本地也需缓存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetClientsResponse> GetClients(GetClientsRequest request, ServerCallContext context)
        {
            var res = new GetClientsResponse();
            var clients = await _mediator.Send(new GetClientQuery()); ;
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
        public override async Task Connect(IAsyncStreamReader<ConnectRequest> requestStream, IServerStreamWriter<ConnectResponse> responseStream, ServerCallContext context)
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

                _logger.LogInformation($"Message from {GetClientIpAddress(context)}");
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // 这里很可能需要修改
                    if (requestStream.MoveNext().Result)
                    {
                        var request = requestStream.Current;
                        // 发起连接信息 中介者处理连接信息
                        await _mediator.Send(new ConnectMessageCommand(request, connectionId));


                        //GrpcMessageHandler.ClientConnectMessageHandler(responseStream, request);

                        // 收到init信息之后才能知晓？

                        // 等待接受一条初始化信息
                        //await responseStream.WriteAsync(new ConnectResponse { Message = $"Hello {request.Info}" });
                    }
                    //await Task.Delay(1000, context.CancellationToken);
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation($"connection: {connectionId} disconnect {ex.Message}");
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
        public override Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileResponse> responseStream, ServerCallContext context)
        {
            return base.DownloadFile(request, responseStream, context);
        }
        /// <summary>
        /// 客户端上传文件至服务端
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            await foreach (var item in requestStream.ReadAllAsync())
            {
                //await _mediator.Send(new UpdateFileCommand(item));
                //Console.WriteLine(item.FileName);
                //Console.WriteLine(item.FileSize);
                //Console.WriteLine(item.FilePath);
                //Console.WriteLine(item.FileData);
            }
            throw new NotImplementedException();

        }
    }
}