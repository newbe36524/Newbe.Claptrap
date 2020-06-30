namespace Newbe.Claptrap.EventNotifier
{
    public class CompoundEventNotifierFactory : IClaptrapComponentFactory<IEventNotifier>
    {
        private readonly CompoundEventNotifier.Factory _factory;

        public CompoundEventNotifierFactory(
            CompoundEventNotifier.Factory factory)
        {
            _factory = factory;
        }

        public IEventNotifier Create(IClaptrapIdentity claptrapIdentity)
        {
            var compoundEventNotifier = _factory.Invoke(claptrapIdentity);
            return compoundEventNotifier;
        }
    }
}