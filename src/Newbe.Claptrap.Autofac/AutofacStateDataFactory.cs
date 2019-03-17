using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class AutofacStateDataFactory : IStateDataFactory
    {
        private readonly IComponentContext _componentContext;

        public AutofacStateDataFactory(
            IActorIdentity actorIdentity,
            IComponentContext componentContext)
        {
            ActorIdentity = actorIdentity;
            _componentContext = componentContext;
        }

        public IActorIdentity ActorIdentity { get; }

        public Task<IStateData> CreateInitialState()
        {
            var key = new DefaultStateDataFactoryRegistrationKey(ActorIdentity.Kind);
            if (_componentContext.TryResolveKeyed(key, typeof(IStateDataFactory),
                    out var service)
                && service is IStateDataFactory factory)
            {
                return factory.CreateInitialState();
            }

            throw new ClaptrapComponentNotFoundException(ActorIdentity, typeof(IStateDataFactory));
        }
    }
}