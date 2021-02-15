using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsEventLoader : IEventLoader
    {
        private readonly IEventLoader _eventLoader;
        private readonly IClaptrapDesign _claptrapDesign;

        public MetricsEventLoader(
            IEventLoader eventLoader,
            IClaptrapDesign claptrapDesign)
        {
            _eventLoader = eventLoader;
            _claptrapDesign = claptrapDesign;
        }

        public IClaptrapIdentity Identity => _eventLoader.Identity;

        public async Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            using var a = ClaptrapActivitySource.Instance
                    .StartActivity(ClaptrapActivitySource.ActivityNames.LoadEvent)!
                .AddClaptrapTags(_claptrapDesign, _eventLoader.Identity);
            using var _ = ClaptrapMetrics.MeasureEventLoader(Identity);
            return await _eventLoader.GetEventsAsync(startVersion, endVersion);
        }
    }
}