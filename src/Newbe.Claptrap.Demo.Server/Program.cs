using System;
using System.Diagnostics;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Demo.Server.Services;
using NLog.Web;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Newbe.Claptrap.Demo.Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
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
                    var config = configBuilder
                        .AddJsonFile(Path.Combine("configs", "appsettings.json"))
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
                .ConfigureServices(services =>
                {
                    services.AddOpenTelemetryTracing(
                        builder => builder
                            .AddAspNetCoreInstrumentation()
                            .AddGrpcClientInstrumentation()
                            .AddHttpClientInstrumentation()
                            .SetSampler(new ParentBasedSampler(new AlwaysOffSampler()))
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("claptrap-host"))
                            .AddSource(ClaptrapActivitySource.Instance.Name)
                            .AddZipkinExporter(options =>
                                options.Endpoint = new Uri("http://localhost:9412/api/v2/spans"))
                    );
                })
                .UseServiceProviderFactory(_ => new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseClaptrapMetrics()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
        }
    }
}