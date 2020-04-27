using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteDbFactory
    {
        SqliteConnection CreateConnection(IActorIdentity actorIdentity);
    }
}