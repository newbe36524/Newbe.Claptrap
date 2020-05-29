using Autofac;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
{
    public class MySqlStorageModule : Autofac.Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "MySql storage module";
        public string Description { get; } = "Module for support event store and state store by using MySql";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);


            builder.RegisterType<MySqlSqlCacheHelper>()
                .As<IMySqlSqlCacheHelper>()
                .SingleInstance();

            // builder.RegisterType<MySqlSharedTableEventEntityLoader>()
            //     .As<IEventEntityLoader<EventEntity>>()
            //     .InstancePerLifetimeScope();
            //
            // builder.RegisterType<MySqlSharedTableEventEntitySaver>()
            //     .As<IEventEntitySaver<EventEntity>>()
            //     .InstancePerLifetimeScope();

            builder.RegisterBuildCallback(scope =>
            {
                var cacheHelper = scope.Resolve<IMySqlSqlCacheHelper>();
                cacheHelper.Init();
            });
        }
    }
}