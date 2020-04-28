using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.SQLite
{
    public class SQLiteDbFactory : ISQLiteDbFactory
    {
        public SqliteConnection CreateConnection(IActorIdentity actorIdentity)
        {
            return new SqliteConnection(DbHelper.ConnectionString(actorIdentity));
        }
    }
}