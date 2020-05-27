using System.Data.SQLite;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteDbFactory
    {
        SQLiteConnection GetStateDbConnection(IClaptrapIdentity claptrapIdentity);
    }
}