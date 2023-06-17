using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciDevHome.Message;
/// <summary>
/// 初始化时更新ClientId(必须是第一条消息) 且9位
/// </summary>
public class ClientIdUpdateMessage
{
    /// <summary>
    /// 客户端
    /// </summary>
    public string ClientId
    {
        get;set;
    }
}
