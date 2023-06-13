## 功能梳理

1. 客户端在服务端注册，并获得id
2. 服务端将客户端的某一个文件夹，作为同步文件夹
3. 客户端也可以主动将某一个文件夹，作为同步文件夹
4. 服务端将同步文件夹的文件，同步到客户端
5. 有一些文件，需要屏蔽，不同步


## 思路
1. 可以是服务端在流中通知，让客户端主动同步文件夹（解耦！）
2. 主客户端（Admin）可以上传到服务端，让服务端同步文件夹

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

