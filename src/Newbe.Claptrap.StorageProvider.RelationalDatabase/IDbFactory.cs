using System.Data;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public interface IDbFactory
    {
        string GetConnectionString(string dbName);
        IDbConnection GetConnection(string dbName);
    }
}