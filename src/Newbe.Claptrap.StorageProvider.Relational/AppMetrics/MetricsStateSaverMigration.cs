using System.Threading.Tasks;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.AppMetrics
{
    public class MetricsStateSaverMigration : IStateSaverMigration
    {
        private readonly IStateSaverMigration _stateSaverMigration;
        private readonly IClaptrapIdentity _identity;

        public MetricsStateSaverMigration(
            IStateSaverMigration stateSaverMigration,
            IClaptrapIdentity identity)
        {
            _stateSaverMigration = stateSaverMigration;
            _identity = identity;
        }

        public async Task MigrateAsync()
        {
            using var _ = ClaptrapMetrics.MeasureStateSaverMigration(_identity);
            await _stateSaverMigration.MigrateAsync();
        }
    }
}