using System.ComponentModel.DataAnnotations.Schema;

namespace SciDevHome.Server.Model
{
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string ClientId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string UserName { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Password { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Mac { get; internal set; }
    }
}
