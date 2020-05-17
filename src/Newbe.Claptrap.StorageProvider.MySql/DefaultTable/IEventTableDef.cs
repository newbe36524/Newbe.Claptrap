namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public interface IEventTableDef
    {
        string SchemaName { get; }
        string EventTableName { get; }
    }
}