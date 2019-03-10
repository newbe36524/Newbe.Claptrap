using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public class AutofacActorFactory : IActorFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public delegate Actor Factory(IActorContext actorContext);

        public AutofacActorFactory(
            ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IActor Create(IActorIdentity identity)
        {
            var actorScope = _lifetimeScope.BeginLifetimeScope(Constants.ActorLifetimeScope);
            var identityProvider = actorScope.Resolve<AutofacActorLifetimeScope>();
            identityProvider.Identity = identity;
            var actorFactory = actorScope.Resolve<Factory>();
            var actorContext = new AutofacActorContext(actorScope.Resolve<IActorContext>(), actorScope);
            var actor = actorFactory.Invoke(actorContext);
            return actor;
        }

        private class AutofacActorContext : IActorContext
        {
            private readonly IActorContext _actorContext;
            private readonly ILifetimeScope _lifetimeScope;
            public IActorIdentity Identity => _actorContext.Identity;

            public IState State => _actorContext.State;

            public Task InitializeAsync()
            {
                return _actorContext.InitializeAsync();
            }

            public Task DisposeAsync()
            {
                _lifetimeScope.Dispose();
                return _actorContext.DisposeAsync();
            }

            public AutofacActorContext(
                IActorContext actorContext,
                ILifetimeScope lifetimeScope)
            {
                _actorContext = actorContext;
                _lifetimeScope = lifetimeScope;
            }
        }
    }
}