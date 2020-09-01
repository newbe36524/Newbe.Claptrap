using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Tests.QuickSetupTools;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Newbe.Claptrap.Tests
{
    public static class QuickSetupTestHelper
    {
        public static IHost BuildHost(
            DatabaseType databaseType,
            RelationLocatorStrategy strategy,
            IEnumerable<string> configJsonFilenames,
            Action<ContainerBuilder> builderAction = null,
            Action<IClaptrapBootstrapperBuilder> bootstrapperAction = null)
        {
            var hostBuilder = new HostBuilder();
            hostBuilder
                .ConfigureServices(collection =>
                {
                    collection.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                        logging.AddNLog();
                    });
                })
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
            var host = hostBuilder.Build();
            return host;
        }
    }
}