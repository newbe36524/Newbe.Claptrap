using Autofac.Features.Indexed;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public class EventStoreFactory : IEventStoreFactory
    {
        private readonly IIndex<EventStoreProvider, IIEventStoreFactoryHandler> _handlers;
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public EventStoreFactory(
            IIndex<EventStoreProvider, IIEventStoreFactoryHandler> handlers,
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _handlers = handlers;
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public IEventStore Create(IActorIdentity identity)
        {
            var provider = _claptrapRegistrationAccessor.FindEventStoreProvider(identity.TypeCode);
            var handler = _handlers[provider];
            var store = handler.Create(identity);
            return store;
        }
    }
}