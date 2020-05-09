namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteDbManager
    {
        void CreateOrUpdateDatabase(IClaptrapIdentity claptrapIdentity);
    }
}