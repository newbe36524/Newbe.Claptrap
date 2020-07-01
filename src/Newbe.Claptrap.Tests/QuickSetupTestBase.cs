using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

namespace Newbe.Claptrap.Tests
{
    [SingleThreaded]
    public abstract class QuickSetupTestBase
    {
        public DatabaseType DatabaseType { get; set; }

        protected QuickSetupTestBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Init();
        }

        protected abstract void Init();

        private IContainer BuildContainer()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"db_configs/claptrap.{DatabaseType:G}.json".ToLower())
                .AddEnvironmentVariables();
            var config = configBuilder.Build();

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
                .AddDefaultConfiguration(config)
                .ScanClaptrapDesigns(new[]
                {
                    typeof(IAccount),
                    typeof(Account),
                    typeof(IAccountMinion),
                    typeof(AccountMinion),
                })
                .ScanClaptrapModule()
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
            const int times = 10;
            const string testId = "HandleEventAsync";
            await using (var lifetimeScope = BuildContainer().BeginLifetimeScope())
            {
                var factory = lifetimeScope.Resolve<Account.Factory>();
                var claptrapIdentity = new ClaptrapIdentity(testId, Codes.Account);
                IAccount account = factory.Invoke(claptrapIdentity);
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
                var claptrapIdentity = new ClaptrapIdentity(testId, Codes.AccountMinion);
                IAccountMinion account = factory.Invoke(claptrapIdentity);
                await account.ActivateAsync();
                var balance = await account.GetBalanceAsync();
                balance.Should().Be(nowBalance);
                Console.WriteLine($"balance from minion {balance}");
            }
        }

        [TestCase("account10", 10)]
        [TestCase("account100", 100)]
        [TestCase("account1000", 1000)]
        [TestCase("account10000", 10000)]
        public async Task SaveEventAsync(string accountId, int count)
        {
            await using var lifetimeScope = BuildContainer().BeginLifetimeScope();
            var factory = (ClaptrapFactory) lifetimeScope.Resolve<IClaptrapFactory>();
            var id = new ClaptrapIdentity(accountId, Codes.Account);
            await using var buildClaptrapLifetimeScope = factory.BuildClaptrapLifetimeScope(id);
            var eventSaver = buildClaptrapLifetimeScope.Resolve<IEventSaver>();
            var tasks = Enumerable.Range(0, count)
                .Select(x => eventSaver.SaveEventAsync(
                    new UnitEvent(id, UnitEvent.TypeCode, new UnitEvent.UnitEventData())
                    {
                        Version = x
                    }));
            await Task.WhenAll(tasks);
        }

        [TestCase("account10", 10)]
        [TestCase("account100", 100)]
        [TestCase("account1000", 1000)]
        [TestCase("account10000", 10000)]
        public async Task SaveStateOneClaptrapAsync(string accountId, int times)
        {
            await using var lifetimeScope = BuildContainer().BeginLifetimeScope();
            var factory = (ClaptrapFactory) lifetimeScope.Resolve<IClaptrapFactory>();
            var id = new ClaptrapIdentity(accountId, Codes.Account);
            await using var buildClaptrapLifetimeScope = factory.BuildClaptrapLifetimeScope(id);
            var eventSaver = buildClaptrapLifetimeScope.Resolve<IStateSaver>();
            var states = Enumerable.Range(0, times)
                .Select(x => new UnitState
                {
                    Data = UnitState.UnitStateData.Create(),
                    Identity = id,
                    Version = x
                })
                .AsParallel()
                .ToArray();
            var tasks = states
                .Select(x => eventSaver.SaveAsync(x));
            await Task.WhenAll(tasks);
        }

        [Theory]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task SaveStateMultipleClaptrapAsync(int claptrapCount)
        {
            await using var lifetimeScope = BuildContainer().BeginLifetimeScope();
            var logger = lifetimeScope.Resolve<ILogger<QuickSetupTestBase>>();
            var factory = (ClaptrapFactory) lifetimeScope.Resolve<IClaptrapFactory>();
            var sw = Stopwatch.StartNew();

            var data = Enumerable.Range(0, claptrapCount)
                .Select(accountId =>
                {
                    var claptrapIdentity = new ClaptrapIdentity($"acc{accountId}", Codes.Account);
                    return new
                    {
                        id = claptrapIdentity,
                        state = new UnitState
                        {
                            Data = UnitState.UnitStateData.Create(),
                            Identity = claptrapIdentity,
                            Version = 100
                        }
                    };
                })
                .AsParallel()
                .ToArray();

            sw.Stop();
            logger.LogInformation("cost {time} ms to build data", sw.ElapsedMilliseconds);
            sw.Restart();

            var lifetimes = data.Select(d =>
                {
                    var id = d.id;
                    var buildClaptrapLifetimeScope = factory.BuildClaptrapLifetimeScope(id);
                    return new
                    {
                        id,
                        d.state,
                        lifetimeScope = buildClaptrapLifetimeScope,
                    };
                })
                .AsParallel()
                .ToArray();

            sw.Stop();
            logger.LogInformation("cost {time} ms to build lifetime", sw.ElapsedMilliseconds);
            sw.Restart();

            var items = lifetimes.Select(d =>
                {
                    var id = d.id;
                    var eventSaver = d.lifetimeScope.Resolve<IStateSaver>();
                    return new
                    {
                        id,
                        d.state,
                        eventSaver,
                        d.lifetimeScope,
                    };
                })
                .AsParallel()
                .ToArray();

            sw.Stop();
            logger.LogInformation("cost {time} ms to build items", sw.ElapsedMilliseconds);
            sw.Restart();

            var tasks = items
                .Select(item => item.eventSaver.SaveAsync(item.state));
            await Task.WhenAll(tasks);
            Parallel.ForEach(items, item => { item.lifetimeScope.Dispose(); });

            sw.Stop();
            logger.LogInformation("cost {time} ms to save items", sw.ElapsedMilliseconds);
        }
    }
}