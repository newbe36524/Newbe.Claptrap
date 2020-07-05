using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Tests.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    [SingleThreaded]
    public abstract class QuickSetupTestBase
    {
        public DatabaseType DatabaseType { get; }
        public RelationLocatorStrategy Strategy { get; }

        protected QuickSetupTestBase(
            DatabaseType databaseType,
            RelationLocatorStrategy strategy)
        {
            DatabaseType = databaseType;
            Strategy = strategy;
            // ReSharper disable once VirtualMemberCallInConstructor
            Init();
        }

        protected virtual IEnumerable<string> AppsettingsFilenames { get; } = Enumerable.Empty<string>();

        protected abstract void Init();

        protected virtual Task OnContainerBuilt(IServiceProvider container)
        {
            return Task.CompletedTask;
        }

        private IServiceProvider ServiceBuilt()
        {
            var container = QuickSetupTestHelper.BuildContainer(
                DatabaseType,
                Strategy,
                AppsettingsFilenames);
            OnContainerBuilt(container).Wait();
            return container;
        }

        [Test]
        public async Task HandleEventAsync()
        {
            decimal oldBalance;
            decimal nowBalance;
            const decimal diff = 100M;
            const int times = 10;
            const string testId = "testId";
            using (var lifetimeScope = ServiceBuilt().CreateScope())
            {
                var factory = lifetimeScope.ServiceProvider.GetRequiredService<Account.Factory>();
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

            using (var lifetimeScope = ServiceBuilt().CreateScope())
            {
                var factory = lifetimeScope.ServiceProvider.GetService<AccountBalanceMinion.Factory>();
                var claptrapIdentity = new ClaptrapIdentity(testId, Codes.AccountBalanceMinion);
                IAccountBalanceMinion accountBalance = factory.Invoke(claptrapIdentity);
                await accountBalance.ActivateAsync();
                var balance = await accountBalance.GetBalanceAsync();
                balance.Should().Be(nowBalance);
                Console.WriteLine($"balance from minion {balance}");
            }
        }

        [TestCase("account10", 10)]
        [TestCase("account100", 100)]
        [TestCase("account1000", 1000)]
        public async Task SaveEventAsync(string accountId, int count)
        {
            using var lifetimeScope = ServiceBuilt().CreateScope();
            var logger = lifetimeScope.ServiceProvider.GetRequiredService<ILogger<QuickSetupTestBase>>();
            var factory = (ClaptrapFactory) lifetimeScope.ServiceProvider.GetRequiredService<IClaptrapFactory>();
            var id = new ClaptrapIdentity(accountId, Codes.Account);
            await using var buildClaptrapLifetimeScope = factory.BuildClaptrapLifetimeScope(id);
            var saver = buildClaptrapLifetimeScope.Resolve<IEventSaver>();
            var tasks = Enumerable.Range(Defaults.EventStartingVersion, count)
                .Select(x => saver.SaveEventAsync(
                    new UnitEvent(id, UnitEvent.TypeCode, new UnitEvent.UnitEventData())
                    {
                        Version = x
                    }));
            await Task.WhenAll(tasks);

            var loader = buildClaptrapLifetimeScope.Resolve<IEventLoader>();
            const int eventBeginVersion = Defaults.StateStartingVersion + 1;
            var eventEndVersion = eventBeginVersion + count;
            var events = await loader.GetEventsAsync(eventBeginVersion, eventEndVersion);
            var versions = events.Select(x => x.Version);
            logger.LogInformation("version from event loader : {version}", versions);
            versions.Should().BeInAscendingOrder()
                .And.OnlyHaveUniqueItems()
                .And.ContainInOrder(Enumerable.Range(Defaults.EventStartingVersion, count));
        }

        [TestCase("account10", 10)]
        [TestCase("account100", 100)]
        [TestCase("account1000", 1000)]
        public async Task SaveStateOneClaptrapAsync(string accountId, int times)
        {
            using var lifetimeScope = ServiceBuilt().CreateScope();
            var factory = (ClaptrapFactory) lifetimeScope.ServiceProvider.GetRequiredService<IClaptrapFactory>();
            var id = new ClaptrapIdentity(accountId, Codes.Account);
            await using var buildClaptrapLifetimeScope = factory.BuildClaptrapLifetimeScope(id);
            var saver = buildClaptrapLifetimeScope.Resolve<IStateSaver>();
            var states = Enumerable.Range(Defaults.StateStartingVersion, times)
                .Select(x => new UnitState
                {
                    Data = UnitState.UnitStateData.Create(),
                    Identity = id,
                    Version = x
                })
                .ToArray();
            var tasks = states
                .Select(x => saver.SaveAsync(x));
            await Task.WhenAll(tasks);

            var loader = buildClaptrapLifetimeScope.Resolve<IStateLoader>();
            var state = await loader.GetStateSnapshotAsync();
            Debug.Assert(state != null, nameof(state) + " != null");
            state.Should().NotBeNull();
            state.Version.Should().Be(times - 1);
        }

        [Theory]
        [TestCase(10)]
        [TestCase(100)]
        public async Task SaveStateMultipleClaptrapAsync(int claptrapCount)
        {
            var stateVersion = 100;
            using var lifetimeScope = ServiceBuilt().CreateScope();
            var logger = lifetimeScope.ServiceProvider.GetRequiredService<ILogger<QuickSetupTestBase>>();
            var factory = (ClaptrapFactory) lifetimeScope.ServiceProvider.GetRequiredService<IClaptrapFactory>();
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
                            Version = stateVersion
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
                .ToArray();

            sw.Stop();
            logger.LogInformation("cost {time} ms to build lifetime", sw.ElapsedMilliseconds);
            sw.Restart();

            var items = lifetimes.Select(d =>
                {
                    var id = d.id;
                    var saver = d.lifetimeScope.Resolve<IStateSaver>();
                    return new
                    {
                        id,
                        d.state,
                        saver,
                        d.lifetimeScope,
                    };
                })
                .ToArray();

            sw.Stop();
            logger.LogInformation("cost {time} ms to build items", sw.ElapsedMilliseconds);
            sw.Restart();

            var tasks = items
                .Select(item => item.saver.SaveAsync(item.state));
            await Task.WhenAll(tasks);

            sw.Stop();
            logger.LogInformation("cost {time} ms to save items", sw.ElapsedMilliseconds);

            foreach (var item in items)
            {
                var loader = item.lifetimeScope.Resolve<IStateLoader>();
                var state = await loader.GetStateSnapshotAsync();
                Debug.Assert(state != null, nameof(state) + " != null");
                state.Version.Should().Be(item.state.Version);
            }

            Parallel.ForEach(items, item => { item.lifetimeScope.Dispose(); });
        }
    }
}