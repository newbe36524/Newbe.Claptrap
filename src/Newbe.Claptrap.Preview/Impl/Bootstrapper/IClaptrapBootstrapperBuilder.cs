using System.Collections.Generic;
using System.Reflection;
using Newbe.Claptrap.Preview.Impl.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IClaptrapBootstrapperBuilder
    {
        IClaptrapBootstrapperBuilder AddAssemblies(IEnumerable<Assembly> assemblies);

        IClaptrapBootstrapperBuilder AddClaptrapDesignStoreConfigurator(IClaptrapDesignStoreConfigurator configurator);

        IClaptrapBootstrapperBuilder AddClaptrapDesignStoreProvider(IClaptrapDesignStoreProvider provider);

        IClaptrapBootstrapper Build();
    }
}