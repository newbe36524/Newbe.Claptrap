using Newbe.Claptrap.StorageProvider.SQLite;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    [Category(nameof(DatabaseType.SQLite))]
    public class SQLiteQuickSetupTest : QuickSetupTestBase
    {
        protected override void Init()
        {
            SQLiteDbFactory.RemoveDataBaseDirectory();
            DatabaseType = DatabaseType.SQLite;
        }
    }

    [Category(nameof(DatabaseType.MySql)), Explicit]
    public class MySqlQuickSetupTest : QuickSetupTestBase
    {
        protected override void Init()
        {
            DatabaseType = DatabaseType.MySql;
        }
    }

    [Category(nameof(DatabaseType.MongoDB)), Explicit]
    public class MongoDBQuickSetupTest : QuickSetupTestBase
    {
        protected override void Init()
        {
            DatabaseType = DatabaseType.MongoDB;
        }
    }

    [Category(nameof(DatabaseType.PostgreSQL)), Explicit]
    public class PostgreSQLQuickSetupTest : QuickSetupTestBase
    {
        protected override void Init()
        {
            DatabaseType = DatabaseType.PostgreSQL;
        }
    }
}