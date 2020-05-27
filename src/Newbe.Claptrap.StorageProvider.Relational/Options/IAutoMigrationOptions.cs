namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IAutoMigrationOptions :
        IStorageProviderOptions
    {
        public bool IsAutoMigrationEnabled { get; }
    }
}