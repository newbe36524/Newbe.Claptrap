using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface IEventStoreMigrationFactory
    {
        IEventLoaderMigration CreateEventLoaderMigration(IClaptrapIdentity identity);
        IEventSaverMigration CreateEventSaverMigration(IClaptrapIdentity identity);
    }
}