using System.Data;
using Npgsql;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL
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
            var re = new NpgsqlConnection(GetConnectionString(connectionName));
            return re;
        }
    }
}