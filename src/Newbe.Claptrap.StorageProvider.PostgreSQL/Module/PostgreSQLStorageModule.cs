using Autofac;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Module
{
    public class PostgreSQLStorageModule : Autofac.Module, IClaptrapAppModule
    {
        public string Name { get; } = "PostgreSQL storage module";
        public string Description { get; } = "Module for support event store and state store by using PostgreSQL";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .SingleInstance();
        }
    }
}