using Autofac;
using Autofac.Extras.AggregateService;

namespace Newbe.Claptrap.Dapr.Modules
{
    public class ClaptrapDaprModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Claptrap orleans module";
        public string Description { get; } = "Module for Orleans support";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapTypeCodeFactory>()
                .As<IClaptrapTypeCodeFactory>()
                .SingleInstance();
           
            builder.RegisterAggregateService<IClaptrapActorCommonService>();
        }
    }
}