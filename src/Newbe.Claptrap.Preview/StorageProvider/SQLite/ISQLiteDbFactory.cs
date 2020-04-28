using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.SQLite
{
    public interface ISQLiteDbFactory
    {
        SqliteConnection CreateConnection(IActorIdentity actorIdentity);
    }
}