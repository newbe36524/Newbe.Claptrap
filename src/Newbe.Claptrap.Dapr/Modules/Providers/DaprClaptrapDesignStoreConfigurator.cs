using System;
using System.Diagnostics;
using System.Linq;

namespace Newbe.Claptrap.Dapr.Modules.Providers
{
    public class DaprClaptrapDesignStoreConfigurator : IClaptrapDesignStoreConfigurator
    {
        private IClaptrapDesignStore? _designStore;

        public void Configure(IClaptrapDesignStore designStore)
        {
            _designStore = designStore;
            AddConfig(
                x => x.ClaptrapOptions.EventCenterOptions == null!,
                x => x.ClaptrapOptions.EventCenterOptions = new EventCenterOptions
                {
                    EventCenterType = EventCenterType.DaprClient
                });
            AddConfig(
                x => x.ClaptrapOptions.EventCenterOptions.EventCenterType == EventCenterType.None,
                x => x.ClaptrapOptions.EventCenterOptions.EventCenterType = EventCenterType.DaprClient);
        }

        private void AddConfig(Func<IClaptrapDesign, bool> predicate,
            Action<IClaptrapDesign> action)
        {
            Debug.Assert(_designStore != null, nameof(_designStore) + " != null");
            foreach (var claptrapDesign in _designStore.Where(predicate))
            {
                action(claptrapDesign);
            }
        }
    }
}