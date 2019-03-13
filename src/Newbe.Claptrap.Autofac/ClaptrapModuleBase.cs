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
            var eventMethodTypes = ImplementAssembly.GetTypes()
                .Where(x => x.Namespace.Contains("EventMethods") && x.IsClass);
            foreach (var eventMethodType in eventMethodTypes)
            {
                builder.RegisterType(eventMethodType)
                    .AsImplementedInterfaces();
            }

            base.Load(builder);
            var assemblies = new[] {InterfaceAssembly, ImplementAssembly};
            builder.RegisterDefaultStateDataFactories(assemblies);
            builder.RegisterUpdateStateDataHandlers(assemblies);
            builder.RegisterMinionEventHandler(assemblies);

            builder.Register(context =>
                    new ActorAssemblyProvider(assemblies))
                .As<IActorAssemblyProvider>();
        }
    }
}