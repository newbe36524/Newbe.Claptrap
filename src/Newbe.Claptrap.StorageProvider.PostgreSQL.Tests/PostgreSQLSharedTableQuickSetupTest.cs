using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
   

    [Category(nameof(DatabaseType.PostgreSQL))]
    [Explicit]
    public class PostgreSQLSharedTableQuickSetupTest : QuickSetupTestBase
    {
        public PostgreSQLSharedTableQuickSetupTest() : base(
            DatabaseType.PostgreSQL,
            RelationLocatorStrategy.SharedTable)
        {
        }

        protected override void Init()
        {
        }
    }
}