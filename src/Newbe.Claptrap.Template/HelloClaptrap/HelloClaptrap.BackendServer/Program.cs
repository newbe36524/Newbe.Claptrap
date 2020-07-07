using HelloClaptrap.Actors.Cart;
using HelloClaptrap.IActor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newbe.Claptrap.Bootstrapper;
using Orleans;

namespace HelloClaptrap.BackendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseClaptrap(builder =>
                {
                    builder
                        .ScanClaptrapDesigns(new[]
                        {
                            typeof(ICartGrain).Assembly,
                            typeof(CartGrain).Assembly
                        });
                })
                .UseOrleansClaptrap()
                .UseOrleans(builder => builder.UseDashboard(options => options.Port = 9000));
    }
}