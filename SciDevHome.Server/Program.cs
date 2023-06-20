using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciDevHome.Server.Model;
using SciDevHome.Server.Services;

namespace SciDevHome.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls($"http://0.0.0.0:{SciDevHomeCommon.DefaultPort}");
            // Add services to the container.
            builder.Services.AddGrpc();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


            builder.Services.AddSingleton<DevHomeService>();
            builder.Services.AddSingleton<StreamGrpcManager>();
            builder.Services.AddDbContext<DevHomeDb>(options =>
                options.UseSqlite(connectionString));

            #region 服务
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<ServerEntryPoint>();
            });
            #endregion


            var app = builder.Build();
            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            
            //new DevHomeDb().Database.Migrate();
            
            // 迁移数据库
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DevHomeDb>();
            dbContext.Database.Migrate();


            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}