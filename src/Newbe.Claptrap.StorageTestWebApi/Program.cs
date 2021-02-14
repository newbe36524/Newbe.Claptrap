using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageTestWebApi.Services;
using NLog.Web;

namespace Newbe.Claptrap.StorageTestWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.AddJsonFile(Path.Combine("configs", "appsettings.json"))
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            var options = new TestConsoleOptions();
            config.Bind(nameof(TestConsoleOptions), options);

            CreateHostBuilder(args, options).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, TestConsoleOptions testConsoleOptions) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(context => new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    var databaseType = testConsoleOptions.DatabaseType;
                    var strategy = RelationLocatorStrategy.SharedTable;
                    configurationBuilder
                        .AddJsonFile("configs/appsettings.json")
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.json".ToLower())
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.{strategy:G}.json".ToLower());

                    configurationBuilder.AddEnvironmentVariables();
                })
                .UseClaptrapMetrics();
    }
}