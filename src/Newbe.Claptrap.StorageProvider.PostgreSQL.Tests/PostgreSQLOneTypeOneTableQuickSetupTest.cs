using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Tests
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