namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public interface IStateTableDef
    {
        string SchemaName { get; }
        string StateTableName { get; }
    }
}