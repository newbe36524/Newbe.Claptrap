using Autofac;
using Microsoft.Extensions.ObjectPool;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.Relational.Module
{
    public class ClaptrapRelationalStorageProviderModule : Autofac.Module, IClaptrapAppModule
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
            builder.RegisterType<StorageMigrationContainer>()
                .As<IStorageMigrationContainer>()
                .SingleInstance();

            builder.RegisterType<DbUpMigration>()
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterType<BatchOperatorContainer>()
                .As<IBatchOperatorContainer>()
                .SingleInstance();
            builder.RegisterGeneric(typeof(ChannelBatchOperator<>))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(MultipleChannelBatchOperator<>))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(ManualBatchOperator<>))
                .AsSelf()
                .InstancePerDependency();
            
            builder.RegisterType<DefaultObjectPoolProvider>()
                .As<ObjectPoolProvider>()
                .SingleInstance()
                .ExternallyOwned();
            builder.Register(t =>
                {
                    var provider = t.Resolve<ObjectPoolProvider>();
                    var objectPool =
                        provider.Create(
                            new BatchItemPooledObjectPolicy());
                    return objectPool;
                })
                .AsSelf()
                .SingleInstance()
                .ExternallyOwned();
        }

        private class BatchItemPooledObjectPolicy : PooledObjectPolicy<MultipleChannelBatchOperator<EventEntity>.BatchItem>
        {
            public override MultipleChannelBatchOperator<EventEntity>.BatchItem Create()
            {
                return new MultipleChannelBatchOperator<EventEntity>.BatchItem
                {
                    Vts = new ManualResetValueTaskSource<int>()
                };
            }

            public override bool Return(MultipleChannelBatchOperator<EventEntity>.BatchItem obj)
            {
                obj.Vts.Reset();
                return true;
            }
        }
    }
}