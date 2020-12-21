using System.Threading.Tasks;
using Dapr.Actors.Runtime;

namespace Newbe.Claptrap.Dapr
{
    public abstract class ClaptrapBoxActor<TStateData> : Actor,
        IClaptrapBoxActor<TStateData>
        where TStateData : IStateData
    {
        private readonly ActorHost _actorService;

        protected ClaptrapBoxActor(
            ActorHost actorService,
            IClaptrapActorCommonService claptrapGrainCommonService) : base(actorService)
        {
            _actorService = actorService;
            ClaptrapGrainCommonService = claptrapGrainCommonService;
        }

        public IClaptrapActorCommonService ClaptrapGrainCommonService { get; }

        public IClaptrap Claptrap =>
            ClaptrapGrainCommonService.ClaptrapAccessor.Claptrap!;

        public TStateData StateData =>
            (TStateData) ClaptrapGrainCommonService.ClaptrapAccessor.Claptrap!.State.Data;

        protected override async Task OnActivateAsync()
        {
            var actorTypeCode = ClaptrapGrainCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var grainActorIdentity = new ClaptrapIdentity(_actorService.Id.GetId(), actorTypeCode);
            var claptrap = ClaptrapGrainCommonService.ClaptrapFactory.Create(grainActorIdentity);
            await claptrap.ActivateAsync();
            ClaptrapGrainCommonService.ClaptrapAccessor.Claptrap = claptrap;
        }

        protected override Task OnDeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}