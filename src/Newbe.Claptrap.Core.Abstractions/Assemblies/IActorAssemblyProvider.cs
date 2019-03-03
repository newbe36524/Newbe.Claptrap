using System.Collections.Generic;
using System.Reflection;

namespace Newbe.Claptrap.Assemblies
{
    public interface IActorAssemblyProvider
    {
        IEnumerable<Assembly> GetAssemblies();
    }
}