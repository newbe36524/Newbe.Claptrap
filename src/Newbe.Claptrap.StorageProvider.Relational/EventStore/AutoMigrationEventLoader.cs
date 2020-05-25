using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class AutoMigrationEventLoader : IEventLoader
    {
        private readonly IEventLoader _eventLoader;
        private readonly Lazy<Task> _migrated;

        public AutoMigrationEventLoader(
            IEventLoader eventLoader,
            IEventLoaderMigration eventLoaderMigration)
        {
            _eventLoader = eventLoader;
            _migrated = new Lazy<Task>(eventLoaderMigration.MigrateAsync);
        }

        public IClaptrapIdentity Identity => _eventLoader.Identity;

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            await _migrated.Value;
            var re = await _eventLoader.GetEventsAsync(startVersion, endVersion);
            return re;
        }
    }
}