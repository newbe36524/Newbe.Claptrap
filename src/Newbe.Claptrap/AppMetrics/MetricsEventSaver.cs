using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsEventSaver : IEventSaver
    {
        private readonly IEventSaver _eventSaver;

        public MetricsEventSaver(
            IEventSaver eventSaver)
        {
            _eventSaver = eventSaver;
        }

        public IClaptrapIdentity Identity => _eventSaver.Identity;

        public async Task SaveEventAsync(IEvent @event)
        {
            using var _ = ClaptrapMetrics.MeasureEventSaver(Identity);
            await _eventSaver.SaveEventAsync(@event);
        }
    }
}