using System.IO;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.StorageTestWebApi.Services;
using Newbe.Claptrap.TestSuit.QuickSetupTools;
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureKestrel((context, serverOptions) =>
                        {
                            // Set properties and call methods on serverOptions
                        });
                })
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
                .UseClaptrap(builder =>
                    {
                        builder.ScanClaptrapDesigns(new[]
                        {
                            typeof(IAccount),
                            typeof(Account),
                            typeof(IAccountBalanceMinion),
                            typeof(AccountBalanceMinion),
                            typeof(IAccountHistoryBalanceMinion),
                            typeof(AccountHistoryBalanceMinion)
                        });
                        builder.ConfigureClaptrapDesign(x =>
                            x.ClaptrapOptions.EventCenterOptions.EventCenterType = EventCenterType.None);
                    },
                    builder =>
                    {
                        builder.RegisterType<Account>()
                            .AsSelf()
                            .InstancePerDependency();
                        builder.RegisterType<AccountBalanceMinion>()
                            .AsSelf()
                            .InstancePerDependency();
                        builder.RegisterModule<StorageSetupModule>();
                        builder.RegisterModule<StorageTestWebApiModule>();
                    })
                .UseOrleansClaptrap()
                .ConfigureServices((host, services) =>
                {
                    services.AddOptions<TestConsoleOptions>()
                        .Configure(
                            consoleOptions => host.Configuration.Bind(nameof(TestConsoleOptions), consoleOptions));
                });
    }
}