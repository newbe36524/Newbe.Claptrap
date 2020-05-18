using System.Data;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public interface IDbFactory
    {
        string GetConnectionString(IClaptrapIdentity identity);
        IDbConnection GetConnection(IClaptrapIdentity identity);
    }
}