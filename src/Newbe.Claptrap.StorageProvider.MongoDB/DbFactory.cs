using MongoDB.Driver;

namespace Newbe.Claptrap.StorageProvider.MongoDB
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

        public IMongoClient GetConnection(string connectionName)
        {
            var mongoClient = new MongoClient(GetConnectionString(connectionName));
            return mongoClient;
        }
    }
}