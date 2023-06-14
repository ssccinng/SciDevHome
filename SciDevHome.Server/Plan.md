## 功能梳理

1. 客户端在服务端注册，并获得id
2. 服务端指定客户端的某一个文件夹，作为同步文件夹（需要客户端反馈），解耦做法，通知客户端，让客户端主动指定一个文件夹作为同步文件夹
3. 客户端也可以主动将某一个文件夹，作为同步文件夹
4. 服务端将同步文件夹的文件，同步到客户端
5. 有一些文件，需要屏蔽，不同步
6. 服务端需要依据同步ID，保留ID与同步文件夹的映射关系，还需要已知客户端的同步文件夹路径吗？还是说，让客户端自己管理
7. 探测 并记录客户端
8. 返回所有客户端的信息
9. 开机自启动
10. 客户端连接上以后发一条初始化信息
11. connectmessage可能要有相应的任务相应信息
12. 能够观察文件信息
13. 每个客户端一个文件夹
14. 查看文件 md5


## 思路
1. 可以是服务端在流中通知，让客户端主动同步文件夹（解耦！）
2. 主客户端（Admin）可以上传到服务端，让服务端同步文件夹
3. 是否两边都要存同步文件夹的路径？（不需要，只需要服务端存储即可？）
4. 用一个固定格式的文件进行同步
5. 并发拷文件

## json命令格式设计
```json
{
    "cmd": "sync",
    "data": {
        "path": "D:\\test",
        "type": "server"
    }
}
```

每种指令提供一个信息样例
1. cmd: sync(同步), 用于通知客户端准备进行同步（并思考同步名在何处）
2. cmd: heart(心跳), 用于客户端向服务端发送心跳
3. cmd: setSyncPath(设置同步路径), 用于客户端向服务端发送同步路径(可能还需要id这类)
4. cmd: getPathInfo, 用于获取客户端的路径信息
4. cmd: pathInfo, 客户端返回的路径信息
5. cmd: clientInit, 用于客户端初始化，发送客户端的信息 传送客户端id等


github chat提供的思路：
```cs
using Grpc.Core;
using System.IO;
using System.Threading.Tasks;

public class MyService : MyService.MyServiceBase
{
    public override async Task SyncFolder(SyncFolderRequest request, IServerStreamWriter<SyncFileResponse> responseStream, ServerCallContext context)
    {
        string folderPath = request.FolderPath;

        // 遍历文件夹中的所有文件，并将它们发送到客户端
        foreach (string filePath in Directory.GetFiles(folderPath))
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await responseStream.WriteAsync(new SyncFileResponse
                    {
                        FileName = Path.GetFileName(filePath),
                        Data = ByteString.CopyFrom(buffer, 0, bytesRead)
                    });
                }
            }
        }
    }
}
```

```cs
using Grpc.Core;
using System.IO;
using System.Threading.Tasks;

public class MyClient
{
    private readonly MyService.MyServiceClient client;

    public MyClient(MyService.MyServiceClient client)
    {
        this.client = client;
    }

    public async Task SyncFolder(string folderPath)
    {
        using (var call = client.SyncFolder(new SyncFolderRequest { FolderPath = folderPath }))
        {
            while (await call.ResponseStream.MoveNext())
            {
                var response = call.ResponseStream.Current;
                string filePath = Path.Combine(folderPath, response.FileName);
                using (FileStream fileStream = File.Create(filePath))
                {
                    await fileStream.WriteAsync(response.Data.ToByteArray(), 0, response.Data.Length);
                }
            }
        }
    }
}
```

