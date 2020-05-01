using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public class ClaptrapDesignStoreFactory : IClaptrapDesignStoreFactory
    {
        private readonly ILogger<ClaptrapDesignStoreFactory> _logger;
        private readonly AttributeBaseClaptrapDesignStoreProvider.Factory _claptrapDesignStoreProviderFactory;
        private readonly IClaptrapDesignStoreCombiner _combiner;
        private readonly List<IClaptrapDesignStoreProvider> _providers;

        public ClaptrapDesignStoreFactory(
            ILogger<ClaptrapDesignStoreFactory> logger,
            AttributeBaseClaptrapDesignStoreProvider.Factory claptrapDesignStoreProviderFactory,
            IClaptrapDesignStoreCombiner combiner)
        {
            _logger = logger;
            _claptrapDesignStoreProviderFactory = claptrapDesignStoreProviderFactory;
            _combiner = combiner;
            _providers = new List<IClaptrapDesignStoreProvider>();
        }

        public IClaptrapDesignStore Create(IEnumerable<Assembly> assemblies)
        {
            var providers = GetProviders().ToArray();
            _logger.LogInformation("start to create claptrap design store from {count} providers : {@providers}",
                providers.Length,
                providers);
            var stores = providers.Select(x => x.Create()).ToArray();
            var claptrapDesignStore = _combiner.Combine(stores);
            _logger.LogInformation("claptrap design store combined by {combiner}", _combiner);
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