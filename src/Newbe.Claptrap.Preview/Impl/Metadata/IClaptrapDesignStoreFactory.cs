using System.Collections.Generic;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public interface IClaptrapDesignStoreFactory
    {
        IClaptrapDesignStore Create(IEnumerable<Assembly> assemblies);

        IClaptrapDesignStoreFactory AddProvider(IClaptrapDesignStoreProvider designStoreProvider);
    }
}