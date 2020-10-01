using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Tests
{
    [Category(nameof(DatabaseType.MongoDB))]
    [Explicit]
    public class MongoDBOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MongoDBOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.MongoDB,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
        }
    }
}