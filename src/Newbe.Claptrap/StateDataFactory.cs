using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap
{
    public abstract class StateDataFactory<TStateData>
        : IStateDataFactory
        where TStateData : class, IStateData
    {
        protected StateDataFactory(IActorIdentity actorIdentity)
        {
            ActorIdentity = actorIdentity;
        }

        public abstract Task<TStateData> Create();

        public IActorIdentity ActorIdentity { get; }

        async Task<IStateData> IStateDataFactory.CreateInitialState()
        {
            var re = await Create();
            return re;
        }
    }
}