using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class NoneStateDataStateDataFactory : IStateDataFactory
    {
        public NoneStateDataStateDataFactory(IActorIdentity actorIdentity)
        {
            ActorIdentity = actorIdentity;
        }

        public IActorIdentity ActorIdentity { get; }

        public Task<IStateData> CreateInitialState()
        {
            return Task.FromResult((IStateData) new NoneStateData());
        }
    }
}