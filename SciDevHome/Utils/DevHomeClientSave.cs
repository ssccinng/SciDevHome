namespace SciDevHome.Utils
{
    public class DevHomeClientSave
    {
        /// <summary>
        /// ID服务器
        /// </summary>
        public string IdServer { get; set; } = "127.0.0.1";
        /// <summary>
        /// 客户端Id
        /// </summary>
        public string ClientId { get; set; } = string.Empty;
        public string ComputerName
        {
            get;
            set;
        } = string.Empty;
    }
}