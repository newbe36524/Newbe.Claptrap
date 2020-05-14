using System.Diagnostics.CodeAnalysis;
using Autofac;
using Newbe.Claptrap.MemoryStore;

namespace Newbe.Claptrap.Modules
{
    [ExcludeFromCodeCoverage]
    public class MemoryStorageModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Memory storage module";
        public string Description { get; } = "Module for registering memory event store and memory state store";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MemoryEventStore>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<MemoryStateStore>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}