using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Saga;

namespace Newbe.Claptrap.Modules
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILoggerFactory _loggerFactory;

        public ClaptrapAppProvider(
            IClaptrapDesignStore claptrapDesignStore,
            ILoggerFactory loggerFactory)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _loggerFactory = loggerFactory;
        }

        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapCustomizationModule(_claptrapDesignStore,
                _loggerFactory.CreateLogger<ClaptrapCustomizationModule>());
            yield return new ToolsModule();
            yield return new NormalBoxModule();
            yield return new ClaptrapFactoryModule();
            yield return new NoChangeStateHolderModule();
            yield return new CompoundEventNotifierModule();
            yield return new SagaClaptrapModule();
        }

        public IEnumerable<IClaptrapDesignStoreConfigurator> GetClaptrapDesignStoreConfigurators()
        {
            yield return new DefaultClaptrapDesignConfigurator();
            // TODO remove to sub module maybe
            yield return new SageClaptrapDesignStoreConfigurator();
        }
    }
}