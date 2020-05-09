using System.Data.SQLite;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteDbFactory
    {
        SQLiteConnection GetEventDbConnection(IClaptrapIdentity claptrapIdentity);
        SQLiteConnection GetStateDbConnection(IClaptrapIdentity claptrapIdentity);
    }
}