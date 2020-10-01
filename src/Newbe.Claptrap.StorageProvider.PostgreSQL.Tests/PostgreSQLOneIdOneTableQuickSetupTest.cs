using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    [Category(nameof(DatabaseType.PostgreSQL))]
    [Explicit]
    public class PostgreSQLOneIdOneTableQuickSetupTest : QuickSetupTestBase
    {
        public PostgreSQLOneIdOneTableQuickSetupTest() : base(
            DatabaseType.PostgreSQL,
            RelationLocatorStrategy.OneIdOneTable)
        {
        }

        protected override void Init()
        {
        }
    }
}