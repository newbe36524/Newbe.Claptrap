using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class SQLiteStateStoreTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SQLiteStateStoreTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task SaveState()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
                builder.RegisterType<SQLiteDbManager>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });
            var actorIdentity = new ActorIdentity(Guid.NewGuid().ToString(), "testCode");
            var noneStateData = new NoneStateData();
            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Serialize(actorIdentity.TypeCode, noneStateData))
                .Returns("123");

            await using var keepConnection = DbHelper.CreateInMemoryConnection(actorIdentity);
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.CreateConnection(actorIdentity))
                .Returns(() => DbHelper.CreateInMemoryConnection(actorIdentity));

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(actorIdentity);
            await sqLiteStateStore.Save(new DataState(actorIdentity, noneStateData, 123));
        }

        [Fact]
        public async Task SaveStateTwice()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
                builder.RegisterType<SQLiteDbManager>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });
            var actorIdentity = new ActorIdentity(Guid.NewGuid().ToString(), "testCode");
            var noneStateData = new NoneStateData();
            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Serialize(actorIdentity.TypeCode, noneStateData))
                .Returns("123");

            await using var keepConnection = DbHelper.CreateInMemoryConnection(actorIdentity);
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.CreateConnection(actorIdentity))
                .Returns(() => DbHelper.CreateInMemoryConnection(actorIdentity));

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(actorIdentity);
            await sqLiteStateStore.Save(new DataState(actorIdentity, noneStateData, 123));
            await sqLiteStateStore.Save(new DataState(actorIdentity, noneStateData, 124));
        }

        [Fact]
        public async Task GetState()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
                builder.RegisterType<SQLiteDbManager>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });
            var actorIdentity = new ActorIdentity(Guid.NewGuid().ToString(), "testCode");
            var noneStateData = new NoneStateData();
            const int version = 123;
            var stateDataString = "123";
            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Serialize(actorIdentity.TypeCode, noneStateData))
                .Returns(stateDataString);

            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Deserialize(actorIdentity.TypeCode, stateDataString))
                .Returns(noneStateData);

            await using var keepConnection = DbHelper.CreateInMemoryConnection(actorIdentity);
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.CreateConnection(actorIdentity))
                .Returns(() => DbHelper.CreateInMemoryConnection(actorIdentity));

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(actorIdentity);
            await sqLiteStateStore.Save(new DataState(actorIdentity, noneStateData, version));

            var stateSnapshot = await sqLiteStateStore.GetStateSnapshot();
            Debug.Assert(stateSnapshot != null, nameof(stateSnapshot) + " != null");
            stateSnapshot.Data.Should().BeOfType<NoneStateData>();
            stateSnapshot.Version.Should().Be(version);
            stateSnapshot.Identity.Id.Should().Be(actorIdentity.Id);
        }
        
        [Fact]
        public async Task NoneState()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMock.GetStrict(builder =>
            {
                builder.AddLogging(_testOutputHelper);
                builder.AddStaticClock(now);
                builder.RegisterType<SQLiteDbManager>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });
            var actorIdentity = new ActorIdentity(Guid.NewGuid().ToString(), "testCode");

            await using var keepConnection = DbHelper.CreateInMemoryConnection(actorIdentity);
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.CreateConnection(actorIdentity))
                .Returns(() => DbHelper.CreateInMemoryConnection(actorIdentity));

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(actorIdentity);

            var stateSnapshot = await sqLiteStateStore.GetStateSnapshot();
            stateSnapshot.Should().BeNull();
        }
    }
}