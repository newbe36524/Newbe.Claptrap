namespace Newbe.Claptrap.Notifiers
{
    public class EmptyEventHandledNotifierFactory : IClaptrapComponentFactory<IEventNotifier>
    {
        private readonly EmptyEventNotifier.Factory _factory;

        public EmptyEventHandledNotifierFactory(
            EmptyEventNotifier.Factory factory)
        {
            _factory = factory;
        }

        public IEventNotifier Create(IClaptrapIdentity claptrapIdentity)
        {
            var re = _factory.Invoke(claptrapIdentity);
            return re;
        }
    }
}