using System.Collections.Generic;
using Autofac;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class AutofacClaptrapBootstrapper : IClaptrapBootstrapper
    {
        private readonly IEnumerable<Module> _modules;

        public AutofacClaptrapBootstrapper(
            IEnumerable<Module> modules)
        {
            _modules = modules;
        }

        public void RegisterServices(ContainerBuilder builder)
        {
            foreach (var module in _modules)
            {
                builder.RegisterModule(module);
            }
        }
    }
}