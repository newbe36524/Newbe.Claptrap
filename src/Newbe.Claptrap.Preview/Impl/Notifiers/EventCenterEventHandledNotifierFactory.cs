using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EventCenterEventHandledNotifierFactory : IClaptrapComponentFactory<IEventHandledNotifier>
    {
        private readonly EventCenterEventHandledNotifier.Factory _factory;

        public EventCenterEventHandledNotifierFactory(
            EventCenterEventHandledNotifier.Factory factory)
        {
            _factory = factory;
        }

        public IEventHandledNotifier Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}