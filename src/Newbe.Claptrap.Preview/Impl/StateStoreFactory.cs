using Autofac.Features.Indexed;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.Metadata;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
{
    public class StateStoreFactory : IStateStoreFactory
    {
        private readonly IIndex<StateStoreProvider, IStateStoreFactoryHandler> _handlers;
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public StateStoreFactory(
            IIndex<StateStoreProvider, IStateStoreFactoryHandler> handlers,
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _handlers = handlers;
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public IStateStore Create(IActorIdentity identity)
        {
            var provider = _claptrapRegistrationAccessor.FindStateStoreProvider(identity.TypeCode);
            var handler = _handlers[provider];
            var store = handler.Create(identity);
            return store;
        }
    }
}