using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Tests
{
 
    [Category(nameof(DatabaseType.MongoDB))]
    [Explicit]
    public class MongoDBSharedTableQuickSetupTest : QuickSetupTestBase
    {
        public MongoDBSharedTableQuickSetupTest() : base(
            DatabaseType.MongoDB,
            RelationLocatorStrategy.SharedTable)
        {
        }

        protected override void Init()
        {
        }
    }
}