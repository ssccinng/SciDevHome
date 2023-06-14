using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciDevHome.Message
{
    public class GetPathRequestMessage
    {
        public string Path { get; set; } = string.Empty;
    }

    public class GetPathResponseMessage
    {
        public string Path { get; set; }
        public List<GrpcDirctoryInfo> GrpcDirctoryInfos { get; set; } = new List<GrpcDirctoryInfo>();
    }
}
