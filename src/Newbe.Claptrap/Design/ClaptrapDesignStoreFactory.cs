using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Design
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

        public IClaptrapDesignStore Create(IEnumerable<Type> types)
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
                yield return _claptrapDesignStoreProviderFactory.Invoke(types);
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