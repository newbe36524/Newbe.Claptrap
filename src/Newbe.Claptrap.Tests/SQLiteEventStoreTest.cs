using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.StorageProvider.SQLite;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class SQLiteEventStoreTest
    {
        [Test]
        public async Task SaveEvent()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMockHelper.Create(
                nowTime: now,
                builderAction: builder =>
                {
                    builder.RegisterType<SQLiteDbManager>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    builder.RegisterType<DbFilePath>()
                        .AsSelf();
                });

            var identity = new TestClaptrapIdentity(Guid.NewGuid().ToString("N"), "testType.Code");
            var eventTypeCode = "eventType";
            var testEventData = new TestEventData();

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Serialize(identity.TypeCode, eventTypeCode, testEventData))
                .Returns("testResult");

            await using var keepConnection = MockDbInMemory(mocker, identity);

            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(identity);
            await db.SaveEventAsync(new DataEvent(identity, eventTypeCode, testEventData));
        }

        [Test]
        public async Task GetEvents()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMockHelper.Create(
                nowTime: now,
                builderAction: builder =>
                {
                    builder.RegisterType<SQLiteDbManager>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    builder.RegisterType<DbFilePath>()
                        .AsSelf();
                });
            var identity = new TestClaptrapIdentity(Guid.NewGuid().ToString("N"), "testType.Code");
            var eventTypeCode = "eventType";
            var testEventData = new TestEventData();

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Serialize(identity.TypeCode, eventTypeCode, testEventData))
                .Returns("testResult");

            mocker.Mock<IEventDataStringSerializer>()
                .Setup(x => x.Deserialize(identity.TypeCode, eventTypeCode, It.IsAny<string>()))
                .Returns(testEventData);

            await using var keepConnection = MockDbInMemory(mocker, identity);

            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(identity);
            var dataEvents = Enumerable.Range(0, 10)
                .Select(i => new DataEvent(identity, eventTypeCode, testEventData)
                {
                    Version = i
                })
                .ToArray();
            foreach (var dataEvent in dataEvents) await db.SaveEventAsync(dataEvent);

            var events = (await db.GetEventsAsync(0, 2)).ToArray();
            var versions = events.Select(x => x.Version).ToArray();
            versions.Contains(0).Should().BeTrue();
            versions.Contains(1).Should().BeTrue();
        }

        private SQLiteConnection MockDbInMemory(AutoMock mocker, IClaptrapIdentity identity)
        {
            var keepConnection = DbHelper.CreateInMemoryConnection(identity);
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.GetEventDbConnection(identity))
                .Returns(() => DbHelper.CreateInMemoryConnection(identity));
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.GetStateDbConnection(identity))
                .Returns(() => DbHelper.CreateInMemoryConnection(identity));
            return keepConnection;
        }

        public class TestEventData : IEventData
        {
            public string Name { get; set; } = "Test";
        }
    }
}