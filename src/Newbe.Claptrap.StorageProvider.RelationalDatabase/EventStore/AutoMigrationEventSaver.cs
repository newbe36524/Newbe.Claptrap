using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore
{
    public class AutoMigrationEventSaver : IEventSaver
    {
        private readonly IEventSaver _eventSaver;
        private readonly Lazy<Task> _migrated;

        public AutoMigrationEventSaver(
            IEventSaver eventSaver,
            IEventStoreMigrationManager eventStoreMigrationManager)
        {
            _eventSaver = eventSaver;
            _migrated = new Lazy<Task>(eventStoreMigrationManager.MigrateAsync);
        }

        public IClaptrapIdentity Identity => _eventSaver.Identity;

        public async Task SaveEventAsync(IEvent @event)
        {
            await _migrated.Value;
            await _eventSaver.SaveEventAsync(@event);
        }
    }
}