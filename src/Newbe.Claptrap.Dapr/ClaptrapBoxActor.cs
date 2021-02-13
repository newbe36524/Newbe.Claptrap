using System.Threading.Tasks;
using Dapr.Actors.Runtime;

namespace Newbe.Claptrap.Dapr
{
    public abstract class ClaptrapBoxActor<TStateData> : Actor,
        IClaptrapBoxActor<TStateData>
        where TStateData : IStateData
    {
        protected ClaptrapBoxActor(
            IClaptrapActorCommonService claptrapActorCommonService)
            : base(claptrapActorCommonService.ActorHost)
        {
            ClaptrapActorCommonService = claptrapActorCommonService;
        }

        public IClaptrapActorCommonService ClaptrapActorCommonService { get; }

        public IClaptrap Claptrap =>
            ClaptrapActorCommonService.ClaptrapAccessor.Claptrap!;

        public TStateData StateData =>
            (TStateData) ClaptrapActorCommonService.ClaptrapAccessor.Claptrap!.State.Data;

        protected override async Task OnActivateAsync()
        {
            var actorTypeCode = ClaptrapActorCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var actorIdentity = new ClaptrapIdentity(ClaptrapActorCommonService.ActorHost.Id.GetId(), actorTypeCode);
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