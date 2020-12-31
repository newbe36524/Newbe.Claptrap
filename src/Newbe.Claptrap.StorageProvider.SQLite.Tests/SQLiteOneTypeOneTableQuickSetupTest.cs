using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.SQLite.Tests
{
    [Category(nameof(DatabaseType.SQLite))]
    [Explicit]
    public class SQLiteOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public SQLiteOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.SQLite,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
            SQLiteDbFactory.SetDataBaseDirectoryName(nameof(SQLiteOneTypeOneTableQuickSetupTest));
            SQLiteDbFactory.RemoveDataBaseDirectory();
        }
    }
}