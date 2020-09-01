using Autofac;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Module
{
    public class MongoDBStorageModule : Autofac.Module, IClaptrapAppModule
    {
        public string Name { get; } = "MongoDB storage module";
        public string Description { get; } = "Module for support event store and state store by using MongoDB";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .SingleInstance();
        }
    }
}