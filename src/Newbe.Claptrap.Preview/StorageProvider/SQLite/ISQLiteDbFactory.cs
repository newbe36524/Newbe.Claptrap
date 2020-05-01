using System.Data.SQLite;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public interface ISQLiteDbFactory
    {
        SQLiteConnection GetEventDbConnection(IClaptrapIdentity claptrapIdentity);
        SQLiteConnection GetStateDbConnection(IClaptrapIdentity claptrapIdentity);
    }
}