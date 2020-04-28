using System.Collections.Generic;
using System.Reflection;

namespace Newbe.Claptrap.Preview
{
    public interface IClaptrapBootstrapperFactory
    {
        IClaptrapBootstrapper Create(IEnumerable<Assembly> assemblies);
    }
}