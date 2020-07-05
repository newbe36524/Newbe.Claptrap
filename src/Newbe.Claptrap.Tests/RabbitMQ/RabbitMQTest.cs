using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newbe.Claptrap.EventCenter;
using Newbe.Claptrap.EventCenter.RabbitMQ;
using Newbe.Claptrap.Tests.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests.RabbitMQ
{
    [Category("RabbitMQ"), Explicit]
    public class RabbitMQTest
    {
        private IEnumerable<string> AppsettingsFilenames
        {
            get { yield return "RabbitMQ/appsettings.json"; }
        }

        [TestCase(CompressType.None)]
        [TestCase(CompressType.Deflate)]
        [TestCase(CompressType.GZip)]
        public async Task Normal(CompressType compressType)
        {
            var random = new Random();
            var minionLocator = new TestMinionLocator();
            var container = QuickSetupTestHelper.BuildContainer(
                DatabaseType.SQLite,
                RelationLocatorStrategy.SharedTable,
                AppsettingsFilenames.Concat(new[]
                    {$"RabbitMQ/appsettings.{compressType.ToString("G").ToLower()}.json"}),
                builder =>
                {
                    builder.RegisterInstance(minionLocator)
                        .As<IMinionLocator>()
                        .SingleInstance();
                });

            var subscriberManager = container.GetRequiredService<IMQSubscriberManager>();
            await subscriberManager.StartAsync();

            var claptrapFactory = (ClaptrapFactory) container.GetRequiredService<IClaptrapFactory>();
            var id1 = new ClaptrapIdentity("1", Codes.Account);
            await using var scope = claptrapFactory.BuildClaptrapLifetimeScope(id1);
            var eventCenter = scope.Resolve<IEventCenter>();
            var eventData = new AccountBalanceChangeEvent
            {
                Diff = random.Next(0, 1000)
            };
            var evt = new DataEvent(id1, Codes.AccountBalanceChangeEvent, eventData)
            {
                Version = 1
            };
            await eventCenter.SendToMinionsAsync(id1, evt);
            await Task.Delay(TimeSpan.FromSeconds(3));

            minionLocator.ConcurrentBag.Count.Should().Be(2);
            var dic = minionLocator.ConcurrentBag.ToDictionary(x => x.Identity);
            var balanceMinionId = new ClaptrapIdentity(id1.Id, Codes.AccountBalanceMinion);
            var balanceMinionItem = dic[balanceMinionId];
            AssertEvent(balanceMinionId, balanceMinionItem);

            var balanceHistoryMinionId = new ClaptrapIdentity(id1.Id, Codes.AccountBalanceMinion);
            var balanceHistoryMinionItem = dic[balanceHistoryMinionId];
            AssertEvent(balanceHistoryMinionId, balanceHistoryMinionItem);

            await subscriberManager.CloseAsync();

            void AssertEvent(ClaptrapIdentity minionId, ReceivedItem item)
            {
                var (id, e) = item;
                id.Should().BeEquivalentTo(minionId);
                e.Version.Should().Be(evt.Version);
                e.EventTypeCode.Should().Be(evt.EventTypeCode);
                e.Data.Should().BeOfType<AccountBalanceChangeEvent>();
                var data = (AccountBalanceChangeEvent) e.Data;
                data.Diff.Should().Be(eventData.Diff);
            }
        }

        [Theory]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        public async Task MultipleSent(int count)
        {
            var container = QuickSetupTestHelper.BuildContainer(
                DatabaseType.SQLite,
                RelationLocatorStrategy.SharedTable,
                AppsettingsFilenames);

            var subscriberManager = container.GetRequiredService<IMQSubscriberManager>();
            await subscriberManager.StartAsync();

            var claptrapFactory = (ClaptrapFactory) container.GetRequiredService<IClaptrapFactory>();
            var id = new ClaptrapIdentity("1", Codes.Account);
            await using var scope = claptrapFactory.BuildClaptrapLifetimeScope(id);
            var eventCenter = scope.Resolve<IEventCenter>();
            var task = Enumerable.Range(0, count)
                .ToObservable()
                .Select(version =>
                {
                    var unitEvent = UnitEvent.Create(id);
                    unitEvent.Version = version;
                    return unitEvent;
                })
                .Select(e => Observable.FromAsync(() => eventCenter.SendToMinionsAsync(id, e)))
                .Merge()
                .ToTask();
            await task;
            await subscriberManager.CloseAsync();
        }


        private class ReceivedItem
        {
            public IClaptrapIdentity Identity { get; set; }
            public IEvent Event { get; set; }

            public void Deconstruct(out IClaptrapIdentity identity, out IEvent @event)
            {
                identity = Identity;
                @event = Event;
            }
        }

        private class TestMinionLocator : IMinionLocator
        {
            public ConcurrentBag<ReceivedItem> ConcurrentBag { get; } = new ConcurrentBag<ReceivedItem>();

            public IMinionProxy CreateProxy(IClaptrapIdentity minionId)
            {
                return new TestMinionProxy(minionId, ConcurrentBag);
            }

            private class TestMinionProxy : IMinionProxy
            {
                private readonly IClaptrapIdentity _minionIdentity;
                private readonly ConcurrentBag<ReceivedItem> _bag;

                public TestMinionProxy(
                    IClaptrapIdentity minionIdentity,
                    ConcurrentBag<ReceivedItem> bag)
                {
                    _minionIdentity = minionIdentity;
                    _bag = bag;
                }

                public Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
                {
                    foreach (var @event in events)
                    {
                        _bag.Add(new ReceivedItem
                        {
                            Event = @event,
                            Identity = _minionIdentity
                        });
                    }

                    return Task.CompletedTask;
                }
            }
        }
    }
}