namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore
{
    public class MongoDBEventStoreLocator : IMongoDBEventStoreLocator
    {
        public string DatabaseName { get; set; } = null!;
        public string ConnectionName { get; set; } = null!;
        public string EventCollectionName { get; set; } = null!;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return FormatString(DatabaseName, identity);
        }

        public string GetDatabaseName(IClaptrapIdentity identity)
        {
            return FormatString(ConnectionName, identity);
        }

        public string GetEventCollectionName(IClaptrapIdentity identity)
        {
            return FormatString(EventCollectionName, identity);
        }

        private static string FormatString(string source, IClaptrapIdentity identity)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var result = source
                .Replace("[Id]", identity.Id)
                .Replace("[TypeCode]", identity.TypeCode);
            return result;
        }
    }
}