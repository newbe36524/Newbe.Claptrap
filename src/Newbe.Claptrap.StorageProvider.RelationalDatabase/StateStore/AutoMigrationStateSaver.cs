using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public class AutoMigrationStateSaver : IStateSaver
    {
        private readonly IStateSaver _stateSaver;
        private readonly Lazy<Task> _migrated;

        public AutoMigrationStateSaver(
            IStateSaver stateSaver,
            IStateStoreMigrationManager stateStoreMigrationManager)
        {
            _stateSaver = stateSaver;
            _migrated = new Lazy<Task>(stateStoreMigrationManager.MigrateAsync);
        }

        public IClaptrapIdentity Identity => _stateSaver.Identity;

        public async Task SaveAsync(IState state)
        {
            await _migrated.Value;
            await _stateSaver.SaveAsync(state);
        }
    }
}