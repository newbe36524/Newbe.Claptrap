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

        public string GetConnectionString(string dbName)
        {
            return _options.StorageConnectionStrings[dbName];
        }

        public IDbConnection GetConnection(string dbName)
        {
            var re = new NpgsqlConnection(GetConnectionString(dbName));
            return re;
        }
    }
}