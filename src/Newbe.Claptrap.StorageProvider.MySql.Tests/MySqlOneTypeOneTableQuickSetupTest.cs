using Newbe.Claptrap.Tests;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.MySql.Tests
{
    [Category(nameof(DatabaseType.MySql))]
    [Explicit]
    public class MySqlOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MySqlOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.MySql,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
        }
    }
}