using System.Collections.Generic;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Design
{
    public interface IClaptrapDesignStoreFactory
    {
        IClaptrapDesignStore Create(IEnumerable<Assembly> assemblies);

        IClaptrapDesignStoreFactory AddProvider(IClaptrapDesignStoreProvider designStoreProvider);
    }
}