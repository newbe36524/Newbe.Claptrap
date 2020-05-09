using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public class EventCenterEventNotifier : IEventNotifier
    {
        public delegate EventCenterEventNotifier Factory(IClaptrapIdentity identity);

        private readonly IEventCenter _eventCenter;

        public EventCenterEventNotifier(
            IClaptrapIdentity identity,
            IEventCenter eventCenter)
        {
            _eventCenter = eventCenter;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task Notify(IEventNotifierContext context)
        {
            return _eventCenter.SendToMinionsAsync(Identity, context.Event);
        }
    }
}