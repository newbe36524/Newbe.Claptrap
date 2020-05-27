using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.StorageProvider.SQLite;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class SQLiteStateStoreTest
    {
        [Test]
        public async Task SaveState()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMockHelper.Create(
                nowTime: now,
                builderAction: builder =>
                {
                    builder.RegisterType<SQLiteDbManager>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var identity = new TestClaptrapIdentity(Guid.NewGuid().ToString(), "testCode");
            var noneStateData = new NoneStateData();
            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Serialize(identity.TypeCode, noneStateData))
                .Returns("123");

            await using var keepConnection = MockDbInMemory(mocker, identity);

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(identity);
            await sqLiteStateStore.SaveAsync(new DataState(identity, noneStateData, 123));
        }

        [Test]
        public async Task SaveStateTwice()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMockHelper.Create(
                nowTime: now,
                builderAction: builder =>
                {
                    builder.RegisterType<SQLiteDbManager>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var identity = new TestClaptrapIdentity(Guid.NewGuid().ToString(), "testCode");
            var noneStateData = new NoneStateData();
            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Serialize(identity.TypeCode, noneStateData))
                .Returns("123");

            await using var keepConnection = MockDbInMemory(mocker, identity);

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(identity);
            await sqLiteStateStore.SaveAsync(new DataState(identity, noneStateData, 123));
            await sqLiteStateStore.SaveAsync(new DataState(identity, noneStateData, 124));
        }

        [Test]
        public async Task GetState()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMockHelper.Create(
                nowTime: now,
                builderAction: builder =>
                {
                    builder.RegisterType<SQLiteDbManager>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var identity = new TestClaptrapIdentity(Guid.NewGuid().ToString(), "testCode");
            var noneStateData = new NoneStateData();
            const int version = 123;
            var stateDataString = "123";
            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Serialize(identity.TypeCode, noneStateData))
                .Returns(stateDataString);

            mocker.Mock<IStateDataStringSerializer>()
                .Setup(x => x.Deserialize(identity.TypeCode, stateDataString))
                .Returns(noneStateData);

            await using var keepConnection = MockDbInMemory(mocker, identity);

            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(identity);
            await sqLiteStateStore.SaveAsync(new DataState(identity, noneStateData, version));

            var stateSnapshot = await sqLiteStateStore.GetStateSnapshotAsync();
            Debug.Assert(stateSnapshot != null, nameof(stateSnapshot) + " != null");
            stateSnapshot.Data.Should().BeOfType<NoneStateData>();
            stateSnapshot.Version.Should().Be(version);
            stateSnapshot.Identity.Id.Should().Be(identity.Id);
        }

        [Test]
        public async Task NoneState()
        {
            var now = DateTime.Parse("2020-01-01");
            using var mocker = AutoMockHelper.Create(
                nowTime: now,
                builderAction: builder =>
                {
                    builder.RegisterType<SQLiteDbManager>()
                        .AsImplementedInterfaces()
                        .SingleInstance();
                });
            var identity = new TestClaptrapIdentity(Guid.NewGuid().ToString(), "testCode");
            await using var keepConnection = MockDbInMemory(mocker, identity);
            var factory = mocker.Create<SQLiteStateStore.Factory>();
            var sqLiteStateStore = factory.Invoke(identity);

            var stateSnapshot = await sqLiteStateStore.GetStateSnapshotAsync();
            stateSnapshot.Should().BeNull();
        }

        private SQLiteConnection MockDbInMemory(AutoMock mocker, IClaptrapIdentity identity)
        {
            var keepConnection = DbHelper.CreateInMemoryConnection(identity);
            mocker.Mock<ISQLiteDbFactory>()
                .Setup(x => x.GetStateDbConnection(identity))
                .Returns(() => DbHelper.CreateInMemoryConnection(identity));
            return keepConnection;
        }
    }
}