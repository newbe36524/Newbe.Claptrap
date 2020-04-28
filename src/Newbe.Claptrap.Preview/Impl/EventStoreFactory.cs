using Autofac.Features.Indexed;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.Metadata;

namespace Newbe.Claptrap.Preview
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