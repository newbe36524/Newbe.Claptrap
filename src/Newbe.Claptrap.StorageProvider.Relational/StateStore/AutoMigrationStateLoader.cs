using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class AutoMigrationStateLoader : IStateLoader
    {
        private readonly IStateLoader _stateLoader;
        private readonly Lazy<Task> _migrated;

        public AutoMigrationStateLoader(
            IStateLoader stateLoader,
            IStateLoaderMigration stateLoaderMigration)
        {
            _stateLoader = stateLoader;
            _migrated = new Lazy<Task>(stateLoaderMigration.MigrateAsync);
        }

        public IClaptrapIdentity Identity => _stateLoader.Identity;

        public async Task<IState?> GetStateSnapshotAsync()
        {
            await _migrated.Value;
            var re = await _stateLoader.GetStateSnapshotAsync();
            return re;
        }
    }
}