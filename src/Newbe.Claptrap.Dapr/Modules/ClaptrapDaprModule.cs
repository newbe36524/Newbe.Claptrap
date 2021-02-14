using Autofac;
using Autofac.Extras.AggregateService;

namespace Newbe.Claptrap.Dapr.Modules
{
    public class ClaptrapDaprModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Claptrap Dapr module";
        public string Description { get; } = "Module for Dapr support";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            builder.RegisterType<ClaptrapTypeCodeFactory>()
                .As<IClaptrapTypeCodeFactory>()
                .SingleInstance();

            builder.RegisterType<DaprMinionActivator>()
                .As<IMinionActivator>()
                .SingleInstance();

            builder.RegisterAggregateService<IClaptrapActorCommonService>();
        }
    }
}