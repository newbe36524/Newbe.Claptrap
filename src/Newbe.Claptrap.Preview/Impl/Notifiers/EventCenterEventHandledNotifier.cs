using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EventCenterEventHandledNotifier : IEventHandledNotifier
    {
        public delegate EventCenterEventHandledNotifier Factory(IClaptrapIdentity identity);

        private readonly IEventCenter _eventCenter;

        public EventCenterEventHandledNotifier(
            IClaptrapIdentity identity,
            IEventCenter eventCenter)
        {
            _eventCenter = eventCenter;
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task Notify(IEventHandledNotifierContext context)
        {
            return _eventCenter.SendToMinionsAsync(Identity, context.Event);
        }
    }
}