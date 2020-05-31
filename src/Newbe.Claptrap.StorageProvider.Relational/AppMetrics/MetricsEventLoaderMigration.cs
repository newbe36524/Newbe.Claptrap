using System.Threading.Tasks;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.Relational.AppMetrics
{
    public class MetricsEventLoaderMigration : IEventLoaderMigration
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IEventLoaderMigration _eventLoaderMigration;

        public MetricsEventLoaderMigration(
            IClaptrapIdentity identity,
            IEventLoaderMigration eventLoaderMigration)
        {
            _identity = identity;
            _eventLoaderMigration = eventLoaderMigration;
        }

        public async Task MigrateAsync()
        {
            using var timer = ClaptrapMetrics.MeasureEventLoaderMigration(_identity);
            await _eventLoaderMigration.MigrateAsync();
        }
    }
}