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
            builder.RegisterGeneric(typeof(ConcurrentListBatchOperator<>))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(ConcurrentListPool<>))
                .As(typeof(IConcurrentListPool<>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(AutoFlushList<>))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterType<StaticAutoFlushListOptions>()
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

            builder.Register(t =>
                {
                    var provider = t.Resolve<ObjectPoolProvider>();
                    var objectPool =
                        provider.Create(
                            new ConcurrentListPooledObjectPolicy<ConcurrentListBatchOperator<EventEntity>.BatchItem>());
                    return objectPool;
                })
                .AsSelf()
                .SingleInstance()
                .ExternallyOwned();
        }

        private class
            BatchItemPooledObjectPolicy : PooledObjectPolicy<ConcurrentListBatchOperator<EventEntity>.BatchItem>
        {
            public override ConcurrentListBatchOperator<EventEntity>.BatchItem Create()
            {
                return new ConcurrentListBatchOperator<EventEntity>.BatchItem
                {
                    Vts = new ManualResetValueTaskSource<int>(),
                };
            }

            public override bool Return(ConcurrentListBatchOperator<EventEntity>.BatchItem obj)
            {
                obj.Vts.Reset();
                return true;
            }
        }

        private class
            ConcurrentListPooledObjectPolicy<T> : PooledObjectPolicy<ConcurrentList<T>>
        {
            public override ConcurrentList<T> Create()
            {
                return new ConcurrentList<T>();
            }

            public override bool Return(ConcurrentList<T> obj)
            {
                obj.ResetIndex();
                return true;
            }
        }
    }
}