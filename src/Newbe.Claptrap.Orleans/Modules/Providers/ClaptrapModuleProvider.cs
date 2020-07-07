using System.Collections.Generic;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapModuleProvider(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            if (design.ClaptrapOptions.EventCenterOptions.EventCenterType == EventCenterType.OrleansClient)
            {
                var re = new OrleansDirectlyEventCenterModule();
                yield return re;
            }
        }
    }
}