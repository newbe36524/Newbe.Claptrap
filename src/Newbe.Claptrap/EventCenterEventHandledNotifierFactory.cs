namespace Newbe.Claptrap
{
    public class EventCenterEventHandledNotifierFactory : IClaptrapComponentFactory<IEventNotifier>
    {
        private readonly EventCenterEventNotifier.Factory _factory;

        public EventCenterEventHandledNotifierFactory(
            EventCenterEventNotifier.Factory factory)
        {
            _factory = factory;
        }

        public IEventNotifier Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}