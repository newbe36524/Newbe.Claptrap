using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public abstract class ClaptrapGrain<TStateData> : Grain, IClaptrapGrain<TStateData>, IGrainWithStringKey
        where TStateData : IStateData
    {
        private readonly IClaptrapGrainCommonService _claptrapGrainCommonService;
        public IActor Actor { get; private set; }
        public TStateData StateData => (TStateData) Actor.State.Data;

        protected ClaptrapGrain(
            IClaptrapGrainCommonService claptrapGrainCommonService)
        {
            _claptrapGrainCommonService = claptrapGrainCommonService;
        }

        public override async Task OnActivateAsync()
        {
            var actorTypeCode = _claptrapGrainCommonService.ActorTypeCodeFactory.GetActorTypeCode(this);
            var grainActorIdentity = new GrainActorIdentity(this.GetPrimaryKeyString(), actorTypeCode);
            Actor = _claptrapGrainCommonService.ActorFactory.Create(grainActorIdentity);
            await Actor.ActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return Actor.DeactivateAsync();
        }
    }
}