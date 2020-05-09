namespace Newbe.Claptrap
{
    public class EventHandlerFactoryFactory : IClaptrapComponentFactory<IEventHandlerFactory>
    {
        private readonly DesignBaseEventHandlerFactory.Factory _factory;

        public EventHandlerFactoryFactory(
            DesignBaseEventHandlerFactory.Factory factory)
        {
            _factory = factory;
        }

        public IEventHandlerFactory Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}