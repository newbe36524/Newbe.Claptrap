using System.Threading.Tasks;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.Relational.AppMetrics
{
    public class MetricsEventSaverMigration : IEventSaverMigration
    {
        private readonly IEventSaverMigration _eventSaverMigration;
        private readonly IClaptrapIdentity _identity;

        public MetricsEventSaverMigration(
            IEventSaverMigration eventSaverMigration,
            IClaptrapIdentity identity)
        {
            _eventSaverMigration = eventSaverMigration;
            _identity = identity;
        }

        public async Task MigrateAsync()
        {
            using var timer = ClaptrapMetrics.MeasureEventSaverMigration(_identity);
            await _eventSaverMigration.MigrateAsync();
        }
    }
}