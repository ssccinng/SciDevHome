using Microsoft.EntityFrameworkCore;

namespace SciDevHome.Server.Model
{
    public class DevHomeDb: DbContext
    {
        public DevHomeDb(DbContextOptions<DevHomeDb> options)
           : base(options)
        {
        }
        public DevHomeDb()
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
