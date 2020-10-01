using Newbe.Claptrap.Tests;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.SQLite.Tests
{
    [Category(nameof(DatabaseType.SQLite))]
    public class SQLiteSharedTableQuickSetupTest : QuickSetupTestBase
    {
        public SQLiteSharedTableQuickSetupTest() : base(
            DatabaseType.SQLite,
            RelationLocatorStrategy.SharedTable)
        {
        }

        protected override void Init()
        {
            SQLiteDbFactory.SetDataBaseDirectoryName(nameof(SQLiteSharedTableQuickSetupTest));
            SQLiteDbFactory.RemoveDataBaseDirectory();
        }
    }
}