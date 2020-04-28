using Autofac.Features.Indexed;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.Autofac
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