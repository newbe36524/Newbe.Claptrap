using System.Data;
using MySql.Data.MySqlClient;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IMySqlDbFactory
    {
        string GetConnectionString(IClaptrapIdentity identity);
        MySqlConnection GetConnection(IClaptrapIdentity identity);
    }
}