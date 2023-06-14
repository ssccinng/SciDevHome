using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciDevHome.Message
{
    public class GrpcDirctoryInfo
    {
        public string Path { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
