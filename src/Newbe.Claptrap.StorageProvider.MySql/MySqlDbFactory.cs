using MySql.Data.MySqlClient;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlDbFactory : IMySqlDbFactory
    {
        public string GetConnectionString(IClaptrapIdentity identity)
        {
            throw new System.NotImplementedException();
        }

        public MySqlConnection GetConnection(IClaptrapIdentity identity)
        {
            throw new System.NotImplementedException();
        }
    }
    
    
}