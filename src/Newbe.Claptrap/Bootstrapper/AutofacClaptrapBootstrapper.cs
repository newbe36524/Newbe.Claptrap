using System;
using System.Collections.Generic;
using Autofac;

namespace Newbe.Claptrap.Bootstrapper
{
    public class AutofacClaptrapBootstrapper : IClaptrapBootstrapper
    {
        private readonly IEnumerable<Module> _modules;
        private readonly Type[] _providers;
        public IClaptrapDesignStore ClaptrapDesignStore { get; }
        public ContainerBuilder Builder { get; set; } = null!;

        public AutofacClaptrapBootstrapper(
            Type[] providers,
            IEnumerable<Module> modules,
            IClaptrapDesignStore claptrapDesignStore)
        {
            _modules = modules;
            _providers = providers;
            ClaptrapDesignStore = claptrapDesignStore;
            Design.ClaptrapDesignStore.Instance = claptrapDesignStore;
        }

        public void Boot()
        {
            Builder.RegisterTypes(_providers)
                .As<IClaptrapModuleProvider>();
            foreach (var module in _modules)
            {
                Builder.RegisterModule(module);
            }
        }

        public void Boot(ContainerBuilder builder)
        {
            Builder = builder;
            Boot();
        }

        public IClaptrapDesignStore DumpDesignStore()
        {
            return ClaptrapDesignStore;
        }
    }
}