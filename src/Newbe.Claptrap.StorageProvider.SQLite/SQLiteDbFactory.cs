using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteDbFactory : ISQLiteDbFactory
    {
        public SqliteConnection CreateConnection(IActorIdentity actorIdentity)
        {
            return new SqliteConnection(DbHelper.ConnectionString(actorIdentity));
        }
    }
}