using Autofac;
using Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Module
{
    public class MongoDBStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "MongoDB storage module";
        public string Description { get; } = "Module for support event store and state store by using MongoDB";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .SingleInstance();

            builder.RegisterType<SharedCollectionEventBatchSaver>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<SharedCollectionEventBatchSaverFactory>()
                .As<ISharedCollectionEventBatchSaverFactory>()
                .SingleInstance();
        }
    }
}