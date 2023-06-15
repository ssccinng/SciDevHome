using Microsoft.EntityFrameworkCore;
using SciDevHome.Data;

namespace SciDevHome.Server.Data
{
    public class DevHomeDB : DbContext
    {
        public DevHomeDB(DbContextOptions options) : base(options)
        {
        }

        public DbSet<HomeUser> Users { get; set; }
    }
}
