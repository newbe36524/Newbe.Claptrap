using System;
using System.Threading.Tasks;
using Orleans;

namespace Newbe.Claptrap.Orleans
{
    public abstract class ClaptrapBoxGrain<TStateData> : Grain,
        IClaptrapBoxGrain<TStateData>
        where TStateData : IStateData
    {
        protected ClaptrapBoxGrain(
            IClaptrapGrainCommonService claptrapGrainCommonService)
        {
            ClaptrapGrainCommonService = claptrapGrainCommonService;
        }

        public IClaptrapGrainCommonService ClaptrapGrainCommonService { get; }

        public IClaptrap Claptrap =>
            ClaptrapGrainCommonService.ClaptrapAccessor.Claptrap!;

        public TStateData StateData =>
            (TStateData) ClaptrapGrainCommonService.ClaptrapAccessor.Claptrap!.State.Data;

        public override async Task OnActivateAsync()
        {
            var actorTypeCode = ClaptrapGrainCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var grainActorIdentity = new ClaptrapIdentity(this.GetPrimaryKeyString(), actorTypeCode);
            var claptrap = ClaptrapGrainCommonService.ClaptrapFactory.Create(grainActorIdentity);
            try
            {
                await claptrap.ActivateAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            ClaptrapGrainCommonService.ClaptrapAccessor.Claptrap = claptrap;
        }

        public override Task OnDeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}