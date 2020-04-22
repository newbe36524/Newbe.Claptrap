using System.Collections.Generic;
using System.Reflection;

namespace Newbe.Claptrap.Autofac
{
    public interface IClaptrapBootstrapperFactory
    {
        IClaptrapBootstrapper Create(IEnumerable<Assembly> assemblies);
    }
}