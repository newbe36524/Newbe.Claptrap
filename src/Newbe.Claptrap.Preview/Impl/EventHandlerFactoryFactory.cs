using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
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