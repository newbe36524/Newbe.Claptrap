using System.Data;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IDbFactory
    {
        string GetConnectionString(string dbName);
        IDbConnection GetConnection(string dbName);
    }
}