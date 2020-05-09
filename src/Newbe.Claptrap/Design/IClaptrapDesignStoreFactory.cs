using System.Collections.Generic;
using System.Reflection;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Design
{
    public interface IClaptrapDesignStoreFactory
    {
        IClaptrapDesignStore Create(IEnumerable<Assembly> assemblies);

        IClaptrapDesignStoreFactory AddProvider(IClaptrapDesignStoreProvider designStoreProvider);
    }
}