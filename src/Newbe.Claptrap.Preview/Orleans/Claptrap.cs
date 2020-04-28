using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Core;
using Orleans;

namespace Newbe.Claptrap.Preview.Orleans
{
    public abstract class Claptrap<TStateData> : Grain, IClaptrap<TStateData>, IGrainWithStringKey
        where TStateData : IStateData
    {
        private readonly IClaptrapGrainCommonService _claptrapGrainCommonService;
        public IActor Actor { get; private set; }
        public TStateData StateData => (TStateData) Actor.State.Data;

        protected Claptrap(
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