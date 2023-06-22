using SciDevHome.Server;

namespace SciDevHome.Providers;


// 连接信息功能提供者
public class ConnectProvider<T>
{
    
    // 要不所有都回复，区别只是收不收
    public bool NeedWait
    {
        get;
        set;
    }
    // public ConnectResponse Handler();
}
// /// <summary>
// /// 获取路径提供者
// /// </summary>
// public class GetPathInfoProvider : ConnectProviderBase
// {
//     
// }