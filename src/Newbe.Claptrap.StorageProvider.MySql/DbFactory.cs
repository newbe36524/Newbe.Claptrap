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

        public string GetConnectionString(string dbName)
        {
            return _options.StorageConnectionStrings[dbName];
        }

        public IDbConnection GetConnection(string dbName)
        {
            var re = new MySqlConnection(GetConnectionString(dbName));
            return re;
        }
    }
}