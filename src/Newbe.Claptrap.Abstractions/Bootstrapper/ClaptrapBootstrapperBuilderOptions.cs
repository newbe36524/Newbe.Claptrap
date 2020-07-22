using System;
using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap
{
    public class ClaptrapBootstrapperBuilderOptions
    {
        /// <summary>
        /// Assemblies to scan claptrap design.
        /// </summary>
        public IEnumerable<Type> DesignTypes { get; set; }
            = Enumerable.Empty<Type>();

        /// <summary>
        /// Assemblies to scan claptrap application module and claptrap module.
        /// </summary>
        public IEnumerable<Type> ModuleTypes { get; set; }
            = Enumerable.Empty<Type>();

        /// <summary>
        /// Claptrap design store provider. Claptrap design store will be built, combined and validated from all providers.
        /// </summary>
        public IList<IClaptrapDesignStoreProvider> ClaptrapDesignStoreProviders { get; set; } =
            new List<IClaptrapDesignStoreProvider>();

        /// <summary>
        /// Configurators for doing some change after a claptrap design store built.
        /// </summary>
        public IList<IClaptrapDesignStoreConfigurator> ClaptrapDesignStoreConfigurators { get; set; } =
            new List<IClaptrapDesignStoreConfigurator>();

        public IList<IClaptrapModuleProvider> ClaptrapModuleProviders { get; set; }
            = new List<IClaptrapModuleProvider>();

        /// <summary>
        /// {ConnectionName:connectionString}
        /// </summary>
        public IDictionary<string, string> StorageConnectionStrings { get; set; }
            = new Dictionary<string, string>();
    }
}