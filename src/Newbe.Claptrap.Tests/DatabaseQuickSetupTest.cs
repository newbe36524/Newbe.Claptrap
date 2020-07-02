using Newbe.Claptrap.StorageProvider.SQLite;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
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
            SQLiteDbFactory.RemoveDataBaseDirectory();
        }
    }

    [Category(nameof(DatabaseType.SQLite))]
    public class SQLiteOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public SQLiteOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.SQLite,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
            SQLiteDbFactory.RemoveDataBaseDirectory();
        }
    }

    [Category(nameof(DatabaseType.SQLite))]
    public class SQLiteOneIdOneTableQuickSetupTest : QuickSetupTestBase
    {
        public SQLiteOneIdOneTableQuickSetupTest() : base(
            DatabaseType.SQLite,
            RelationLocatorStrategy.OneIdOneTable)
        {
        }

        protected override void Init()
        {
            SQLiteDbFactory.RemoveDataBaseDirectory();
        }
    }

    [Category(nameof(DatabaseType.MySql)), Explicit]
    public class MySqlSharedTableQuickSetupTest : QuickSetupTestBase
    {
        public MySqlSharedTableQuickSetupTest() : base(
            DatabaseType.MySql,
            RelationLocatorStrategy.SharedTable)
        {
        }

        protected override void Init()
        {
        }
    }

    [Category(nameof(DatabaseType.MySql)), Explicit]
    public class MySqlOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MySqlOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.MySql,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
        }
    }

    [Category(nameof(DatabaseType.MySql)), Explicit]
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

    [Category(nameof(DatabaseType.MongoDB)), Explicit]
    public class MongoDBSharedTableQuickSetupTest : QuickSetupTestBase
    {
        public MongoDBSharedTableQuickSetupTest() : base(
            DatabaseType.MongoDB,
            RelationLocatorStrategy.SharedTable)
        {
        }

        protected override void Init()
        {
        }
    }

    [Category(nameof(DatabaseType.MongoDB)), Explicit]
    public class MongoDBOneTypeOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MongoDBOneTypeOneTableQuickSetupTest() : base(
            DatabaseType.MongoDB,
            RelationLocatorStrategy.OneTypeOneTable)
        {
        }

        protected override void Init()
        {
        }
    }

    [Category(nameof(DatabaseType.MongoDB)), Explicit]
    public class MongoDBOneIdOneTableQuickSetupTest : QuickSetupTestBase
    {
        public MongoDBOneIdOneTableQuickSetupTest() : base(
            DatabaseType.MongoDB,
            RelationLocatorStrategy.OneIdOneTable)
        {
        }

        protected override void Init()
        {
        }
    }

    [Category(nameof(DatabaseType.PostgreSQL)), Explicit]
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

    [Category(nameof(DatabaseType.PostgreSQL)), Explicit]
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

    [Category(nameof(DatabaseType.PostgreSQL)), Explicit]
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