namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.Options
{
    public interface IAutoMigrationOptions :
        IEventSaverOptions,
        IEventLoaderOptions,
        IStateSaverOptions,
        IStateLoaderOptions
    {
        public bool Enabled { get; }
    }
}