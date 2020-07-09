namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class RelationalStateStoreLocator : IRelationalStateStoreLocator
    {
        public string SchemaName { get; set; } = Defaults.SchemaName;
        public string ConnectionName { get; set; } = Defaults.ConnectionName;
        public string StateTableName { get; set; } = Defaults.StateTableName;

        public string GetConnectionName(IClaptrapIdentity identity)
        {
            return FormatString(ConnectionName, identity);
        }

        public string GetSchemaName(IClaptrapIdentity identity)
        {
            return FormatString(SchemaName, identity);
        }

        public string GetStateTableName(IClaptrapIdentity identity)
        {
            return FormatString(StateTableName, identity);
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