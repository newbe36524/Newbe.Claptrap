using Newbe.Claptrap.Tests;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Tests
{
    [Category(nameof(DatabaseType.MongoDB))]
    [Explicit]
    public class MongoDBOneIdOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MongoDBOneIdOneTableQuickSetupTest() : base(
            DatabaseType.MongoDB,
            RelationLocatorStrategy.OneIdOneTable)
        {
        }

        protected override void Init()
        {
        }
    }
}