using Autofac;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview
{
    public class ActorFactory : IActorFactory
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IStateStoreFactory _stateStoreFactory;
        private readonly IEventStoreFactory _eventStoreFactory;

        public ActorFactory(
            ILifetimeScope lifetimeScope,
            IStateStoreFactory stateStoreFactory,
            IEventStoreFactory eventStoreFactory)
        {
            _lifetimeScope = lifetimeScope;
            _stateStoreFactory = stateStoreFactory;
            _eventStoreFactory = eventStoreFactory;
        }

        public IActor Create(IActorIdentity identity)
        {
            var actorScope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.Register(context => _stateStoreFactory.Create(identity))
                    .As<IStateStore>()
                    .SingleInstance();
                builder.Register(context => _eventStoreFactory.Create(identity))
                    .As<IEventStore>()
                    .SingleInstance();
                builder.Register(context => identity)
                    .AsSelf()
                    .SingleInstance();
            });
            var actor = actorScope.Resolve<Actor>();
            return actor;
        }
    }
}