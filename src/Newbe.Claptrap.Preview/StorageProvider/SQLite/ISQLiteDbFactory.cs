using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public interface ISQLiteDbFactory
    {
        SqliteConnection CreateConnection(IClaptrapIdentity claptrapIdentity);
    }
}