namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public class SharedStateTableDef : IStateTableDef
    {
        public delegate SharedStateTableDef Factory();

        public string SchemaName { get; } = "claptrap";
        public string StateTableName { get; } = "claptrap_state_shared";
    }
}