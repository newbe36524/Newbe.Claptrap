using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IEventStoreMigrationFactory
    {
        IEventLoaderMigration CreateEventLoaderMigration(IClaptrapIdentity identity);
        IEventSaverMigration CreateEventSaverMigration(IClaptrapIdentity identity);
    }
}