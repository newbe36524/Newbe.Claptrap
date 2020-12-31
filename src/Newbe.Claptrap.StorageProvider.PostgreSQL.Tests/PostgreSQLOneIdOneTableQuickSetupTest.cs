using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Tests
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