using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.Dapr
{
    public abstract class ClaptrapBoxActor<TStateData> : Actor,
        IClaptrapBoxActor<TStateData>
        where TStateData : IStateData
    {
        private readonly ActorHost _actorHost;

        protected ClaptrapBoxActor(ActorHost actorHost,
            IClaptrapActorCommonService claptrapActorCommonService)
            : base(actorHost)
        {
            _actorHost = actorHost;
            ClaptrapActorCommonService = claptrapActorCommonService;
        }

        public IClaptrapActorCommonService ClaptrapActorCommonService { get; }

        public IClaptrap Claptrap =>
            ClaptrapActorCommonService.ClaptrapAccessor.Claptrap!;

        public TStateData StateData =>
            (TStateData) ClaptrapActorCommonService.ClaptrapAccessor.Claptrap!.State.Data;

        public IEventSerializer<EventJsonModel> EventSerializer =>
            ClaptrapActorCommonService.EventSerializer;

        protected override async Task OnActivateAsync()
        {
            var actorTypeCode = ClaptrapActorCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var actorIdentity = new ClaptrapIdentity(_actorHost.Id.GetId(), actorTypeCode);
            var claptrap = ClaptrapActorCommonService.ClaptrapFactory.Create(actorIdentity);
            await claptrap.ActivateAsync();
            ClaptrapActorCommonService.ClaptrapAccessor.Claptrap = claptrap;
        }

        protected override Task OnDeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}