using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl;
using Orleans;

namespace Newbe.Claptrap.Preview.Orleans
{
    public abstract class ClaptrapBox<TStateData> : Grain, IClaptrapBox<TStateData>, IClaptrapGrain
        where TStateData : IStateData
    {
        private readonly IClaptrapGrainCommonService _claptrapGrainCommonService;
        public IClaptrap Claptrap { get; private set; } = null!;
        public TStateData StateData => (TStateData) Claptrap.State.Data;

        protected ClaptrapBox(
            IClaptrapGrainCommonService claptrapGrainCommonService)
        {
            _claptrapGrainCommonService = claptrapGrainCommonService;
        }

        public override async Task OnActivateAsync()
        {
            var actorTypeCode = _claptrapGrainCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var grainActorIdentity = new ClaptrapIdentity(this.GetPrimaryKeyString(), actorTypeCode);
            Claptrap = _claptrapGrainCommonService.ClaptrapFactory.Create(grainActorIdentity);
            await Claptrap.ActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}