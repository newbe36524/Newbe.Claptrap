using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsEventSaver : IEventSaver
    {
        private readonly IEventSaver _eventSaver;
        private readonly IClaptrapDesign _claptrapDesign;

        public MetricsEventSaver(
            IEventSaver eventSaver,
            IClaptrapDesign claptrapDesign)
        {
            _eventSaver = eventSaver;
            _claptrapDesign = claptrapDesign;
        }

        public IClaptrapIdentity Identity => _eventSaver.Identity;

        public async Task SaveEventAsync(IEvent @event)
        {
            using var a = ClaptrapActivitySource.Instance
                    .StartActivity(ClaptrapActivitySource.ActivityNames.SaveEvent)!
                .AddClaptrapTags(_claptrapDesign, _eventSaver.Identity);
            using var _ = ClaptrapMetrics.MeasureEventSaver(Identity);
            await _eventSaver.SaveEventAsync(@event);
        }
    }
}