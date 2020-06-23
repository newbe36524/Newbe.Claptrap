using System.Threading.Tasks;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.AppMetrics
{
    public class MetricsStateLoaderMigration : IStateLoaderMigration
    {
        private readonly IStateLoaderMigration _stateLoaderMigration;
        private readonly IClaptrapIdentity _identity;

        public MetricsStateLoaderMigration(
            IStateLoaderMigration stateLoaderMigration,
            IClaptrapIdentity identity)
        {
            _stateLoaderMigration = stateLoaderMigration;
            _identity = identity;
        }

        public async Task MigrateAsync()
        {
            using var _ = ClaptrapMetrics.MeasureStateLoaderMigration(_identity);
            await _stateLoaderMigration.MigrateAsync();
        }
    }
}