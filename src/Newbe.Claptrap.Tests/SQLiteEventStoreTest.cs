using Autofac.Extras.Moq;
using FluentAssertions;
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
        public void Test()
        {
            var mocker = AutoMock.GetStrict(builder => { builder.AddLogging(_testOutputHelper); });
            var factory = mocker.Create<SQLiteEventStore.Factory>();
            var db = factory.Invoke(new ActorIdentity("123", "testType.Code"));
            db.DeleteDatabaseIfFound();
            db.CreateOrUpdateDatabase();
            db.Should().NotBeNull();
        }
    }
}