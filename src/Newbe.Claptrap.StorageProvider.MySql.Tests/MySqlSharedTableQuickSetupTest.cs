using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.MySql.Tests
{
    [Category(nameof(DatabaseType.MySql))]
    [Explicit]
    public class MySqlSharedTableQuickSetupTest : QuickSetupTestBase
    {
        public MySqlSharedTableQuickSetupTest() : base(
            DatabaseType.MySql,
            RelationLocatorStrategy.SharedTable)
        {
        }

        protected override void Init()
        {
        }
    }
}