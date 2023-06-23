using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciDevHome.Message
{
    /// <summary>
    /// connect信息
    /// </summary>
    public class GetPathRequestMessage
    {
        public string Path { get; set; } = string.Empty;
    }

    public class GetPathResponseMessage
    {
        // 可能加入返回值类型？？
        public string Path { get; set; }
        public List<GrpcDirctoryInfo> GrpcDirctoryInfos { get; set; } = new List<GrpcDirctoryInfo>();
    }
}
