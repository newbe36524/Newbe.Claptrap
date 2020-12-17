using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.TestSuit.QuickSetupTools;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Newbe.Claptrap.TestSuit
{
    public static class QuickSetupTestHelper
    {
        public static IHost BuildHost(
            DatabaseType databaseType,
            RelationLocatorStrategy strategy,
            IEnumerable<string> configJsonFilenames,
            Action<ContainerBuilder> builderAction = null,
            Action<IClaptrapBootstrapperBuilder> bootstrapperAction = null,
            Action<HostBuilder> configureHostBuilder = null)
        {
            var hostBuilder = new HostBuilder();
            hostBuilder
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder
                        .AddJsonFile("configs/appsettings.json")
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.json".ToLower())
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.{strategy:G}.json".ToLower());
                    foreach (var filename in configJsonFilenames)
                    {
                        configurationBuilder.AddJsonFile(filename);
                    }

                    configurationBuilder.AddEnvironmentVariables();
                })
                .ConfigureServices(collection =>
                {
                    collection.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                        logging.AddNLog();
                    });
                })
                .UseClaptrap(bootstrapperBuilder =>
                {
                    bootstrapperBuilder.ScanClaptrapDesigns(new[]
                    {
                        typeof(IAccount),
                        typeof(Account),
                        typeof(IAccountBalanceMinion),
                        typeof(AccountBalanceMinion),
                        typeof(IAccountHistoryBalanceMinion),
                        typeof(AccountHistoryBalanceMinion)
                    });
                    bootstrapperBuilder.ConfigureClaptrapDesign(x =>
                        x.ClaptrapOptions.EventCenterOptions.EventCenterType = EventCenterType.None);
                    bootstrapperAction?.Invoke(bootstrapperBuilder);
                }, containerBuilder =>
                {
                    builderAction?.Invoke(containerBuilder);
                    containerBuilder.RegisterType<Account>()
                        .AsSelf()
                        .InstancePerDependency();
                    containerBuilder.RegisterType<AccountBalanceMinion>()
                        .AsSelf()
                        .InstancePerDependency();
                });
            configureHostBuilder?.Invoke(hostBuilder);
            var host = hostBuilder.Build();
            return host;
        }
    }
}