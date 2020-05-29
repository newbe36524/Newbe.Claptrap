using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Module
{
    public class ClaptrapRelationalStorageProviderModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap relational database storage provider model";
        public string Description { get; } = "Module for sharing components in relation database storage providers";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqlTemplateCache>()
                .As<ISqlTemplateCache>()
                .SingleInstance();

            builder.RegisterType<EventEntityMapper>()
                .As<IEventEntityMapper<EventEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StateEntityMapper>()
                .As<IStateEntityMapper<StateEntity>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RelationalStoreFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}