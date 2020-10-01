using Newbe.Claptrap.Tests;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.SQLite.Tests
{
    [Category(nameof(DatabaseType.SQLite))]
    [Explicit]
    public class SQLiteOneIdOneTableQuickSetupTest : QuickSetupTestBase
    {
        public SQLiteOneIdOneTableQuickSetupTest() : base(
            DatabaseType.SQLite,
            RelationLocatorStrategy.OneIdOneTable)
        {
        }

        protected override void Init()
        {
            SQLiteDbFactory.SetDataBaseDirectoryName(nameof(SQLiteOneIdOneTableQuickSetupTest));
            SQLiteDbFactory.RemoveDataBaseDirectory();
        }
    }
}