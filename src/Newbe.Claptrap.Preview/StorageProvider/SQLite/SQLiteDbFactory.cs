using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public class SQLiteDbFactory : ISQLiteDbFactory
    {
        public SqliteConnection CreateConnection(IClaptrapIdentity claptrapIdentity)
        {
            return new SqliteConnection(DbHelper.ConnectionString(claptrapIdentity));
        }
    }
}