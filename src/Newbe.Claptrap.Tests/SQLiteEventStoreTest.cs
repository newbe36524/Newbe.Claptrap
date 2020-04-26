using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class SQLiteEventStoreTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SQLiteEventStoreTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateDatabase()
        {
            var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(new ActorIdentity("123", "testType.Code"));
            db.DeleteDatabaseIfFound();
            db.CreateOrUpdateDatabase();
            db.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveEvent()
        {
            var now = DateTime.Parse("2020-01-01");
            var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
            });
            var actorIdentity = new ActorIdentity("123", "testType.Code");
            var eventTypeCode = "eventType";
            var testEventData = new TestEventData();

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Serialize(actorIdentity.TypeCode, eventTypeCode, testEventData))
                .Returns("testResult");

            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(actorIdentity);
            db.DeleteDatabaseIfFound();
            db.CreateOrUpdateDatabase();
            var eventSavingResult =
                await db.SaveEvent(new DataEvent(actorIdentity, eventTypeCode, testEventData, null));
            eventSavingResult.Should().Be(EventSavingResult.Success);
        }

        [Fact]
        public async Task SaveEventTwice()
        {
            var now = DateTime.Parse("2020-01-01");
            var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
            });
            var actorIdentity = new ActorIdentity("123", "testType.Code");
            var eventTypeCode = "eventType";
            var testEventData = new TestEventData();

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Serialize(actorIdentity.TypeCode, eventTypeCode, testEventData))
                .Returns("testResult");

            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(actorIdentity);
            db.DeleteDatabaseIfFound();
            db.CreateOrUpdateDatabase();
            var eventSavingResult =
                await db.SaveEvent(new DataEvent(actorIdentity, eventTypeCode, testEventData, null));
            eventSavingResult.Should().Be(EventSavingResult.Success);
            eventSavingResult =
                await db.SaveEvent(new DataEvent(actorIdentity, eventTypeCode, testEventData, null));
            eventSavingResult.Should().Be(EventSavingResult.AlreadyAdded);
        }


        [Fact]
        public async Task GetEvents()
        {
            var now = DateTime.Parse("2020-01-01");
            var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
            });
            var actorIdentity = new ActorIdentity("123", "testType.Code");
            var eventTypeCode = "eventType";
            var testEventData = new TestEventData();

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Serialize(actorIdentity.TypeCode, eventTypeCode, testEventData))
                .Returns("testResult");

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Deserialize(actorIdentity.TypeCode, eventTypeCode, It.IsAny<string>()))
                .Returns(testEventData);

            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(actorIdentity);
            db.DeleteDatabaseIfFound();
            db.CreateOrUpdateDatabase();
            var dataEvents = Enumerable.Range(0, 10)
                .Select(i => new DataEvent(actorIdentity, eventTypeCode, testEventData, Guid.NewGuid().ToString())
                {
                    Version = i
                })
                .ToArray();
            foreach (var dataEvent in dataEvents)
            {
                await db.SaveEvent(dataEvent);
            }

            var events = (await db.GetEvents(0, 2)).ToArray();
            var versions = events.Select(x => x.Version).ToArray();
            versions.Contains(0).Should().BeTrue();
            versions.Contains(1).Should().BeTrue();
        }

        public class TestEventData : IEventData
        {
            public string Name { get; set; } = "Test";
        }
    }
}