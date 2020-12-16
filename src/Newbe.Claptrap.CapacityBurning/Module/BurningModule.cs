using Autofac;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.CapacityBurning.Services;
using Newbe.Claptrap.StorageSetup;

namespace Newbe.Claptrap.CapacityBurning.Module
{
    public class BurningModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule<StorageSetupModule>();
            builder.RegisterType<ClaptrapFactory>()
                .AsImplementedInterfaces()
                .AsSelf();
            builder.RegisterType<EventSavingBurningService>()
                .AsSelf();
            builder.RegisterType<StateSavingBurningService>()
                .AsSelf();

            MethodTimeLogger.LogLevel = LogLevel.Information;
        }
    }
}