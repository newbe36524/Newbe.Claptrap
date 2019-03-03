using System.Collections.Generic;
using System.Reflection;
using Newbe.Claptrap.Assemblies;

namespace Newbe.Claptrap.Autofac
{
    public class ActorAssemblyProvider : IActorAssemblyProvider
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public ActorAssemblyProvider(
            IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }
}