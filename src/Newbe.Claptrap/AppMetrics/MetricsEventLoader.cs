using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsEventLoader : IEventLoader
    {
        private readonly IEventLoader _eventLoader;

        public MetricsEventLoader(
            IEventLoader eventLoader)
        {
            _eventLoader = eventLoader;
        }

        public IClaptrapIdentity Identity => _eventLoader.Identity;

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            using var _ = ClaptrapMetrics.MeasureEventLoader(Identity);
            return await _eventLoader.GetEventsAsync(startVersion, endVersion);
        }
    }
}