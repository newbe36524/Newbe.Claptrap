using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Logging;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public class ClaptrapDesignStoreFactory : IClaptrapDesignStoreFactory
    {
        private readonly ILog Logger = LogProvider.For<ClaptrapDesignStoreFactory>();
        private readonly AttributeBaseClaptrapDesignStoreProvider.Factory _claptrapDesignStoreProviderFactory;
        private readonly IClaptrapDesignStoreCombiner _combiner;
        private readonly List<IClaptrapDesignStoreProvider> _providers;

        public ClaptrapDesignStoreFactory(
            AttributeBaseClaptrapDesignStoreProvider.Factory claptrapDesignStoreProviderFactory,
            IClaptrapDesignStoreCombiner combiner)
        {
            _claptrapDesignStoreProviderFactory = claptrapDesignStoreProviderFactory;
            _combiner = combiner;
            _providers = new List<IClaptrapDesignStoreProvider>();
        }

        public IClaptrapDesignStore Create(IEnumerable<Assembly> assemblies)
        {
            var providers = GetProviders().ToArray();
            Logger.Info("start to create claptrap design store from {count} providers : {@providers}",
                providers.Length,
                providers);
            var stores = providers.Select(x => x.Create()).ToArray();
            var claptrapDesignStore = _combiner.Combine(stores);
            Logger.Info("claptrap design store combined by {combiner}", _combiner);
            return claptrapDesignStore;

            IEnumerable<IClaptrapDesignStoreProvider> GetProviders()
            {
                yield return _claptrapDesignStoreProviderFactory.Invoke(assemblies.SelectMany(x => x.DefinedTypes));
                foreach (var claptrapDesignStoreProvider in _providers)
                {
                    yield return claptrapDesignStoreProvider;
                }
            }
        }

        public IClaptrapDesignStoreFactory AddProvider(IClaptrapDesignStoreProvider designStoreProvider)
        {
            _providers.Add(designStoreProvider);
            return this;
        }
    }
}