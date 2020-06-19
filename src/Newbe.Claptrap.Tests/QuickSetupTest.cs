using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Tests.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class QuickSetupTest
    {
        private static IContainer BuildContainer()
        {
            var services = new ServiceCollection();
            services.AddLogging(logging => { logging.SetMinimumLevel(LogLevel.Debug); });
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
                .ScanClaptrapDesigns(new[]
                {
                    typeof(IAccount),
                    typeof(Account),
                    typeof(IAccountMinion),
                    typeof(AccountMinion),
                })
                .ScanClaptrapModule()
                .UseSQLiteAsTestingStorage()
                .Build();
            claptrapBootstrapper.Boot();

            var container = containerBuilder.Build();
            return container;
        }

        [Test]
        public async Task HandleEventAsync()
        {
            decimal oldBalance;
            decimal nowBalance;
            const decimal diff = 100M;
            var times = 10;
            await using (var lifetimeScope = BuildContainer().BeginLifetimeScope())
            {
                var factory = lifetimeScope.Resolve<Account.Factory>();
                var id = new ClaptrapIdentity("1", Codes.Account);
                IAccount account = factory.Invoke(id);
                await account.ActivateAsync();
                oldBalance = await account.GetBalanceAsync();
                await Task.WhenAll(Enumerable.Range(0, times)
                    .Select(i => account.ChangeBalanceAsync(diff)));
                var balance = await account.GetBalanceAsync();
                await account.DeactivateAsync();
                nowBalance = oldBalance + times * 100;
                balance.Should().Be(nowBalance);
            }

            Console.WriteLine($"balance change: {oldBalance} + {diff} = {nowBalance}");

            await using (var lifetimeScope = BuildContainer().BeginLifetimeScope())
            {
                var factory = lifetimeScope.Resolve<AccountMinion.Factory>();
                var id = new ClaptrapIdentity("1", Codes.AccountMinion);
                IAccountMinion account = factory.Invoke(id);
                await account.ActivateAsync();
                var balance = await account.GetBalanceAsync();
                balance.Should().Be(nowBalance);
                Console.WriteLine($"balance from minion {balance}");
            }
        }
    }
}