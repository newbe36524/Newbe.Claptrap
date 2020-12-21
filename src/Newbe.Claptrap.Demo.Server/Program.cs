using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Dapr.Actors.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Demo.Server.Services;
using NLog.Web;

namespace Newbe.Claptrap.Demo.Server
{
    internal class Program
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    var configBuilder = new ConfigurationBuilder();
                    var config = configBuilder.AddJsonFile(Path.Combine("configs", "appsettings.json"))
                        .AddEnvironmentVariables()
                        .Build();
                    var testConsoleOptions = new TestConsoleOptions();
                    config.Bind(nameof(TestConsoleOptions), testConsoleOptions);
                    var databaseType = testConsoleOptions.DatabaseType;
                    var strategy = RelationLocatorStrategy.SharedTable;
                    configurationBuilder
                        .AddJsonFile("configs/appsettings.json")
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.json".ToLower())
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.{strategy:G}.json".ToLower());

                    configurationBuilder.AddEnvironmentVariables();
                })
                .UseClaptrap(builder => { builder.ScanClaptrapDesigns(new[] {typeof(AccountGrain).Assembly}); })
                .UseDaprClaptrap()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseActors(options =>
                        {
                            options.Actors.RegisterActor<AccountGrain>();
                        });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
        }
    }
}