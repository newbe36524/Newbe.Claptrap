namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public class SharedTableEventTableDef : IEventTableDef
    {
        public delegate SharedTableEventTableDef Factory();

        public string SchemaName { get; } = "claptrap";
        public string EventTableName { get; } = "claptrap_event_shared";
    }
}