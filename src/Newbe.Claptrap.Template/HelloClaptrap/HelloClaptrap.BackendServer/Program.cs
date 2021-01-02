using System;
using System.Net;
using Dapr.Actors.AspNetCore;
using HelloClaptrap.Actors.Cart;
using HelloClaptrap.IActor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using NLog.Web;

namespace HelloClaptrap.BackendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseClaptrap(typeof(ICartGrain).Assembly, typeof(CartActor).Assembly)
                .UseClaptrapHostCommon()
                .UseClaptrapDaprHost()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureKestrel((context, options) =>
                        {
                            var httpPort = context.Configuration.GetValue("PORT", 80);
                            options.Listen(IPAddress.Any, httpPort,
                                listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                        })
                        .UseActors(options => { })
                        ;
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}