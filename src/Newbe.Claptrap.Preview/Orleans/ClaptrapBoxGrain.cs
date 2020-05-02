using System.Diagnostics;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Box;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl;
using Orleans;

namespace Newbe.Claptrap.Preview.Orleans
{
    public abstract class ClaptrapBoxGrain<TStateData> : Grain,
        IClaptrapBoxGrain<TStateData>,
        IClaptrapGrain
        where TStateData : IStateData
    {
        private IClaptrapBox? _box;

        protected ClaptrapBoxGrain(
            IClaptrapGrainCommonService claptrapGrainCommonService)
        {
            ClaptrapGrainCommonService = claptrapGrainCommonService;
        }

        public IClaptrapGrainCommonService ClaptrapGrainCommonService { get; }

        public IClaptrap Claptrap
        {
            get
            {
                Debug.Assert(_box != null, nameof(_box) + " != null");
                return _box.Claptrap;
            }
        }

        public TStateData StateData
        {
            get
            {
                Debug.Assert(_box != null, nameof(_box) + " != null");
                return (TStateData) _box.Claptrap.State.Data;
            }
        }

        public override async Task OnActivateAsync()
        {
            var actorTypeCode = ClaptrapGrainCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var grainActorIdentity = new ClaptrapIdentity(this.GetPrimaryKeyString(), actorTypeCode);
            var box = ClaptrapGrainCommonService.BoxFactory.Create(grainActorIdentity);
            await box.Claptrap.ActivateAsync();
            _box = box;
        }

        public override Task OnDeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}