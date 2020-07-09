using System.Data;
using MySql.Data.MySqlClient;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class DbFactory : IDbFactory
    {
        private readonly ClaptrapBootstrapperBuilderOptions _options;

        public DbFactory(
            ClaptrapBootstrapperBuilderOptions options)
        {
            _options = options;
        }

        public string GetConnectionString(string connectionName)
        {
            return _options.StorageConnectionStrings[connectionName];
        }

        public IDbConnection GetConnection(string connectionName)
        {
            var re = new MySqlConnection(GetConnectionString(connectionName));
            return re;
        }
    }
}