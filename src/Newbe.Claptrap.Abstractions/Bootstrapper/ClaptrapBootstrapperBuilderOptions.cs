using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Newbe.Claptrap.Bootstrapper
{
    public class ClaptrapBootstrapperBuilderOptions
    {
        /// <summary>
        /// Culture info about claptrap exception message and logging.
        /// If not be set, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = null!;

        /// <summary>
        /// Assemblies to scan claptrap design.
        /// </summary>
        public IEnumerable<Assembly> DesignAssemblies { get; set; }
            = Enumerable.Empty<Assembly>();

        /// <summary>
        /// Assemblies to scan claptrap application module and claptrap module.
        /// </summary>
        public IEnumerable<Assembly> ModuleAssemblies { get; set; }
            = Enumerable.Empty<Assembly>();

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
    }
}