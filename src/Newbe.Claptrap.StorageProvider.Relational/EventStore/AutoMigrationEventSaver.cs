using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class AutoMigrationEventSaver : IEventSaver
    {
        private readonly IEventSaver _eventSaver;
        private readonly Lazy<Task> _migrated;

        public AutoMigrationEventSaver(
            IEventSaver eventSaver,
            IEventSaverMigration eventSaverMigration)
        {
            _eventSaver = eventSaver;
            _migrated = new Lazy<Task>(eventSaverMigration.MigrateAsync);
        }

        public IClaptrapIdentity Identity => _eventSaver.Identity;

        public async Task SaveEventAsync(IEvent @event)
        {
            await _migrated.Value;
            await _eventSaver.SaveEventAsync(@event);
        }
    }
}