using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Tests.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests.LoadConfiguration
{
    public class ConfigurationTest
    {
        private static IContainer BuildContainer(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddConsole();
            });
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterType<Account>()
                .AsSelf()
                .InstancePerDependency();
            containerBuilder.RegisterType<AccountMinion>()
                .AsSelf()
                .InstancePerDependency();
            var builder = new AutofacClaptrapBootstrapperBuilder(new NullLoggerFactory(), containerBuilder);
            var claptrapBootstrapper = builder
                .ScanClaptrapModule()
                .AddDefaultConfiguration(configuration)
                .ScanClaptrapDesigns(new[]
                {
                    typeof(IAccount),
                    typeof(Account),
                    typeof(IAccountMinion),
                    typeof(AccountMinion),
                })
                .Build();
            claptrapBootstrapper.Boot();

            var container = containerBuilder.Build();
            return container;
        }

        [TestCase("sqlite")]
        [TestCase("mongodb")]
        [TestCase("mysql")]
        [TestCase("postgresql")]
        [TestCase("multiple_db")]
        public void LoadConfigurationFromJson(string jsonFileName)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile($"LoadConfiguration/{jsonFileName}.json");
            var configuration = builder.Build();
            var container = BuildContainer(configuration);
            var scope = container.BeginLifetimeScope();
            var factory = scope.Resolve<Account.Factory>();
            var account = factory.Invoke(new ClaptrapIdentity("1", Codes.Account));
            account.Should().NotBeNull();
        }
    }
}