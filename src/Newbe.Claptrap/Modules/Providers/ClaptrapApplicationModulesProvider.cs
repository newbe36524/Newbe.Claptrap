using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Modules
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILoggerFactory _loggerFactory;

        public ClaptrapApplicationModulesProvider(
            IClaptrapDesignStore claptrapDesignStore,
            ILoggerFactory loggerFactory)
        {
            _claptrapDesignStore = claptrapDesignStore;
            _loggerFactory = loggerFactory;
        }

        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
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
    }
}