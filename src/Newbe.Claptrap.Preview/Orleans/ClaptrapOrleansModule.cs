using Autofac;
using Autofac.Extras.AggregateService;
using Newbe.Claptrap.Preview.Abstractions;

namespace Newbe.Claptrap.Preview.Orleans
{
    public class ClaptrapOrleansModule : Module
    {
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