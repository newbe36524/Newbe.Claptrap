using System.Collections.Generic;
using Autofac;

namespace Newbe.Claptrap.Bootstrapper
{
    public class AutofacClaptrapBootstrapper : IClaptrapBootstrapper
    {
        private readonly IEnumerable<Module> _modules;
        private readonly ContainerBuilder _builder;
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public AutofacClaptrapBootstrapper(
            ContainerBuilder builder,
            IEnumerable<Module> modules,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _modules = modules;
            _builder = builder;
            _claptrapDesignStore = claptrapDesignStore;
        }

        public void Boot()
        {
            foreach (var module in _modules)
            {
                _builder.RegisterModule(module);
            }
        }

        public IClaptrapDesignStore DumpDesignStore()
        {
            return _claptrapDesignStore;
        }
    }
}