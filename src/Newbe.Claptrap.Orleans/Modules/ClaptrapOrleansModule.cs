using Autofac;
using Autofac.Extras.AggregateService;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class ClaptrapOrleansModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap orleans module";
        public string Description { get; } = "Module for Orleans support";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapTypeCodeFactory>()
                .As<IClaptrapTypeCodeFactory>()
                .SingleInstance();
            builder.RegisterType<OrleansMinionActivator>()
                .As<IMinionActivator>()
                .SingleInstance();
            builder.RegisterAggregateService<IClaptrapGrainCommonService>();
        }
    }
}