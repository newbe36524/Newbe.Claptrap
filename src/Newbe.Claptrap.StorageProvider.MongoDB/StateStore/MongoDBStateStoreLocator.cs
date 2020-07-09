namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore
{
    public class MongoDBStateStoreLocator : IMongoDBStateStoreLocator
    {
        public string DatabaseName { get; set; } = null!;
        public string ConnectionName { get; set; } = null!;
        public string StateCollectionName { get; set; } = null!;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return FormatString(DatabaseName, identity);
        }

        public string GetDatabaseName(IClaptrapIdentity identity)
        {
            return FormatString(ConnectionName, identity);
        }

        public string GetStateCollectionName(IClaptrapIdentity identity)
        {
            return FormatString(StateCollectionName, identity);
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