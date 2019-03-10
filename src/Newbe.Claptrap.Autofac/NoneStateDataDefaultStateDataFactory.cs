using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class NoneStateDataDefaultStateDataFactory : IDefaultStateDataFactory
    {
        public NoneStateDataDefaultStateDataFactory(IActorIdentity actorIdentity)
        {
            ActorIdentity = actorIdentity;
        }

        public IActorIdentity ActorIdentity { get; }

        public Task<IStateData> Create()
        {
            return Task.FromResult((IStateData) new NoneStateData());
        }
    }
}