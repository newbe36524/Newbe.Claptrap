using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class AutofacDefaultStateDataFactory : IDefaultStateDataFactory
    {
        private readonly IComponentContext _componentContext;

        public AutofacDefaultStateDataFactory(
            IActorIdentity actorIdentity,
            IComponentContext componentContext)
        {
            ActorIdentity = actorIdentity;
            _componentContext = componentContext;
        }

        public IActorIdentity ActorIdentity { get; }

        public Task<IStateData> Create()
        {
            var key = new DefaultStateDataFactoryRegistrationKey(ActorIdentity.Kind);
            if (_componentContext.TryResolveKeyed(key, typeof(IDefaultStateDataFactory),
                    out var service)
                && service is IDefaultStateDataFactory factory)
            {
                return factory.Create();
            }

            throw new ClaptrapComponentNotFoundException(ActorIdentity, typeof(IDefaultStateDataFactory));
        }
    }
}