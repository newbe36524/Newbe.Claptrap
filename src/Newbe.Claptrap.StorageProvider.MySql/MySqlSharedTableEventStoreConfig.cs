namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlSharedTableEventStoreConfig
    {
        public string SchemaName { get; set; } = "claptrap";
        public string EventTableName { get; set; } = "claptrap_event_shared";
        public string SharedTableEventStoreDbName { get; set; }
    }
}