using System.Collections.Generic;
using Autofac;

namespace Newbe.Claptrap.Preview
{
    public class AutofacClaptrapBootstrapper : IClaptrapBootstrapper
    {
        private readonly IEnumerable<Autofac.Module> _modules;

        public AutofacClaptrapBootstrapper(
            IEnumerable<Autofac.Module> modules)
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