using System.Collections.Generic;
using Autofac;
using Force.DeepCloner;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class AutofacClaptrapBootstrapper : IClaptrapBootstrapper
    {
        private readonly IEnumerable<Module> _modules;
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public AutofacClaptrapBootstrapper(
            IEnumerable<Module> modules,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _modules = modules;
            _claptrapDesignStore = claptrapDesignStore.DeepClone();
        }

        public void RegisterServices(ContainerBuilder builder)
        {
            foreach (var module in _modules)
            {
                builder.RegisterModule(module);
            }
        }

        public IClaptrapDesignStore DumpDesignStore()
        {
            return _claptrapDesignStore;
        }
    }
}