using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.TestSuit.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.TestSuit
{
    [SingleThreaded]
    public abstract class QuickSetupTestBase
    {
        public DatabaseType DatabaseType { get; }
        public RelationLocatorStrategy Strategy { get; }
        public IHost Host { get; set; }

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

        protected virtual Task OnStopHost(IHost host)
        {
            return Task.CompletedTask;
        }

        private IServiceProvider BuildService()
        {
            var host = QuickSetupTestHelper.BuildHost(
                DatabaseType,
                Strategy,
                AppsettingsFilenames);
            Host = host;
            OnContainerBuilt(host.Services).Wait();
            return host.Services;
        }

        [Test]
        public async Task HandleEventAsync()
        {
            decimal oldBalance;
            decimal nowBalance;
            const decimal diff = 100M;
            const int times = 10;
            const string testId = "testId";
            using (var lifetimeScope = BuildService().CreateScope())
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

            using (var lifetimeScope = BuildService().CreateScope())
            {
                var factory = lifetimeScope.ServiceProvider.GetService<AccountBalanceMinion.Factory>();
                var claptrapIdentity = new ClaptrapIdentity(testId, Codes.AccountBalanceMinion);
                IAccountBalanceMinion accountBalance = factory.Invoke(claptrapIdentity);
                await accountBalance.ActivateAsync();
                var balance = await accountBalance.GetBalanceAsync();
                balance.Should().Be(nowBalance);
                Console.WriteLine($"balance from minion {balance}");
            }

            await OnStopHost(Host);
            await Host.StopAsync();
        }

        [TestCase(10, 10, true)]
        [TestCase(100, 10, false)]
        // [TestCase(1000, 10, false)]
        public async Task SaveEventAsync(int actorCount, int count, bool validateByLoader)
        {
            using var lifetimeScope = BuildService().CreateScope();
            var logger = lifetimeScope.ServiceProvider.GetRequiredService<ILogger<QuickSetupTestBase>>();
            var factory = (ClaptrapFactory) lifetimeScope.ServiceProvider.GetRequiredService<IClaptrapFactory>();

            var showTimeIndex = actorCount / 10;
            showTimeIndex = showTimeIndex == 0 ? 1 : showTimeIndex;
            var round = 1;
            var tasks = Enumerable.Range(0, actorCount)
                .Select(async actorId =>
                {
                    var index = Interlocked.Increment(ref round);
                    var accountId = $"acc_{actorId}_{actorCount}_{count}_{index}";
                    var id = new ClaptrapIdentity(accountId, Codes.Account);
                    await using var scope = factory.BuildClaptrapLifetimeScope(id);
                    var saver = scope.Resolve<IEventSaver>();
                    var sourceEvent = UnitEvent.Create(id);
                    var unitEvents = Enumerable.Range(Defaults.EventStartingVersion, count)
                        .Select(x => sourceEvent with {Version = x})
                        .ToArray();
                    var sw = Stopwatch.StartNew();
                    foreach (var unitEvent in unitEvents)
                    {
                        await saver.SaveEventAsync(unitEvent);
                    }

                    sw.Stop();
                    if (actorId / showTimeIndex == 0)
                    {
                        Console.WriteLine($"cost {sw.ElapsedMilliseconds} ms to save event");
                    }

                    if (validateByLoader)
                    {
                        var loader = scope.Resolve<IEventLoader>();
                        const int eventBeginVersion = Defaults.StateStartingVersion + 1;
                        var eventEndVersion = eventBeginVersion + count;
                        var events = await loader.GetEventsAsync(eventBeginVersion, eventEndVersion);
                        var versions = events.Select(x => x.Version);
                        logger.LogInformation("version from event loader : {version}", versions);
                        versions.Should().BeInAscendingOrder()
                            .And.OnlyHaveUniqueItems()
                            .And.ContainInOrder(Enumerable.Range(Defaults.EventStartingVersion, count));
                    }
                });
            await tasks.WhenAllComplete(actorCount);

            await OnStopHost(Host);
            await Host.StopAsync();
        }

        [TestCase("account10", 10)]
        [TestCase("account100", 100)]
        [TestCase("account1000", 1000)]
        public async Task SaveStateOneClaptrapAsync(string accountId, int times)
        {
            using var lifetimeScope = BuildService().CreateScope();
            var factory = (ClaptrapFactory) lifetimeScope.ServiceProvider.GetRequiredService<IClaptrapFactory>();
            var id = new ClaptrapIdentity(accountId, Codes.Account);
            await using var buildClaptrapLifetimeScope = factory.BuildClaptrapLifetimeScope(id);
            var saver = buildClaptrapLifetimeScope.Resolve<IStateSaver>();
            var states = Enumerable.Range(Defaults.StateStartingVersion, times)
                .Select(x => new DataState(id, new AccountState
                {
                    Balance = 666M
                }, x))
                .ToArray();
            await states
                .Select(x => saver.SaveAsync(x))
                .WhenAllComplete(states.Length);

            var loader = buildClaptrapLifetimeScope.Resolve<IStateLoader>();
            var state = await loader.GetStateSnapshotAsync();
            Debug.Assert(state != null, nameof(state) + " != null");
            state.Should().NotBeNull();
            await OnStopHost(Host);
            await Host.StopAsync();
        }

        [Theory]
        [TestCase(10)]
        [TestCase(100)]
        public async Task SaveStateMultipleClaptrapAsync(int claptrapCount)
        {
            const int stateVersion = 100;
            using var lifetimeScope = BuildService().CreateScope();
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
                        state = new DataState(claptrapIdentity, new AccountState
                        {
                            Balance = 666M
                        }, stateVersion)
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
                        lifetimeScope = buildClaptrapLifetimeScope
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
                        d.lifetimeScope
                    };
                })
                .ToArray();

            sw.Stop();
            logger.LogInformation("cost {time} ms to build items", sw.ElapsedMilliseconds);
            sw.Restart();

            await items
                .Select(item => item.saver.SaveAsync(item.state))
                .WhenAllComplete(items.Length);

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
            await OnStopHost(Host);
            await Host.StopAsync();
        }
    }
}