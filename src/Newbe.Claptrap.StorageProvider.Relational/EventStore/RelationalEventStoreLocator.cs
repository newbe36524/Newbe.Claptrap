namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class RelationalEventStoreLocator : IRelationalEventStoreLocator
    {
        public string SchemaName { get; set; } = Defaults.SchemaName;
        public string ConnectionName { get; set; } = Defaults.ConnectionName;
        public string EventTableName { get; set; } = Defaults.EventTableName;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return FormatString(ConnectionName, identity);
        }

        public string GetSchemaName(IClaptrapIdentity identity)
        {
            return FormatString(SchemaName, identity);
        }

        public string GetEventTableName(IClaptrapIdentity identity)
        {
            return FormatString(EventTableName, identity);
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