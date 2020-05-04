using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newbe.Claptrap.Preview.Impl.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class ClaptrapBootstrapperBuilderOptions
    {
        /// <summary>
        /// Culture info about claptrap exception message and logging.
        /// If not be set, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Assemblies to scan claptrap design.
        /// </summary>
        public IEnumerable<Assembly> ScanningAssemblies { get; set; }

        /// <summary>
        /// Claptrap design store provider. Claptrap design store will be built, combined and validated from all providers.
        /// </summary>
        public IList<IClaptrapDesignStoreProvider> ClaptrapDesignStoreProviders { get; set; }

        /// <summary>
        /// Configurators for doing some change after a claptrap design store built.
        /// </summary>
        public IList<IClaptrapDesignStoreConfigurator> ClaptrapDesignStoreConfigurators { get; set; }
    }
}