using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    [Category(nameof(DatabaseType.PostgreSQL))]
    [Explicit]
    public class PostgreSQLOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public PostgreSQLOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.PostgreSQL,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
        }
    }
}