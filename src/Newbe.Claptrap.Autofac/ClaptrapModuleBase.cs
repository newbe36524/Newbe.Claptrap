using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Newbe.Claptrap.Assemblies;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Autofac
{
    public abstract class ClaptrapModuleBase : Module
    {
        protected abstract Assembly InterfaceAssembly { get; }
        protected abstract Assembly ImplementAssembly { get; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assemblies = new[] {InterfaceAssembly, ImplementAssembly};
            builder.RegisterEventMethods(assemblies);
            builder.RegisterDefaultStateDataFactories(assemblies);
            builder.RegisterUpdateStateDataHandlers(assemblies);
            builder.RegisterMinionEventHandler(assemblies);

            builder.Register(context =>
                    new ActorAssemblyProvider(assemblies))
                .As<IActorAssemblyProvider>();
        }
    }
}