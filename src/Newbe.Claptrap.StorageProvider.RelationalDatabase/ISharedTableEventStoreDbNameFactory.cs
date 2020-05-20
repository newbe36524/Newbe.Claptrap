namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public interface ISharedTableEventStoreDbNameFactory
    {
        string GetDbName();
    }
}