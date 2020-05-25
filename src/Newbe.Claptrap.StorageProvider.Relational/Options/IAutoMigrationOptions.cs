namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IAutoMigrationOptions :
        IEventSaverOptions,
        IEventLoaderOptions,
        IStateSaverOptions,
        IStateLoaderOptions
    {
        public bool IsAutoMigrationEnabled { get; }
    }
}