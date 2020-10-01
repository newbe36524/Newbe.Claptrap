using Newbe.Claptrap.Tests;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.MySql.Tests
{
    [Category(nameof(DatabaseType.MySql))]
    [Explicit]
    public class MySqlOneIdOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MySqlOneIdOneTableQuickSetupTest() : base(
            DatabaseType.MySql,
            RelationLocatorStrategy.OneIdOneTable)
        {
        }

        protected override void Init()
        {
        }
    }
}