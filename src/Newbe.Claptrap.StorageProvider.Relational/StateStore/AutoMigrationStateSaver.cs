using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class AutoMigrationStateSaver : IStateSaver
    {
        private readonly IStateSaver _stateSaver;
        private readonly Lazy<Task> _migrated;

        public AutoMigrationStateSaver(
            IStateSaver stateSaver,
            IStateSaverMigration stateSaverMigration)
        {
            _stateSaver = stateSaver;
            _migrated = new Lazy<Task>(stateSaverMigration.MigrateAsync);
        }

        public IClaptrapIdentity Identity => _stateSaver.Identity;

        public async Task SaveAsync(IState state)
        {
            await _migrated.Value;
            await _stateSaver.SaveAsync(state);
        }
    }
}