using Autofac;
using Microsoft.Extensions.Logging;

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

            MethodTimeLogger.LogLevel = LogLevel.Information;
        }
    }
}