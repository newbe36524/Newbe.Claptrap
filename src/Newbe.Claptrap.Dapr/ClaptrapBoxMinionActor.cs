using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.Dapr
{
    public abstract class ClaptrapBoxMinionActor<TStateData> : ClaptrapBoxActor<TStateData>,
        IClaptrapMinionActor
        where TStateData : IStateData
    {
        protected ClaptrapBoxMinionActor(ActorHost actorHost, IClaptrapActorCommonService claptrapActorCommonService) :
            base(actorHost, claptrapActorCommonService)
        {
        }

        public virtual async Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                await Claptrap.HandleEventAsync(@event);
            }
        }

        public virtual async Task MasterEventReceivedJsonAsync(IEnumerable<EventJsonModel> events)
        {
            var items = events.Select(ClaptrapActorCommonService.EventSerializer.Deserialize);
            foreach (var @event in items)
            {
                await Claptrap.HandleEventAsync(@event);
            }
        }

        public virtual Task WakeAsync()
        {
            return Task.CompletedTask;
        }
    }
}