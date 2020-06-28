using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.CapacityBurning.Services;

namespace Newbe.Claptrap.CapacityBurning.Module
{
    public class BurningModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapFactory>()
                .AsImplementedInterfaces()
                .AsSelf();
            builder.RegisterType<EventSavingBurningService>()
                .AsSelf();
            builder.RegisterType<StateSavingBurningService>()
                .AsSelf();
            builder.RegisterType<DockerComposeService>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<DataBaseService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            MethodTimeLogger.LogLevel = LogLevel.Information;
        }
    }
}