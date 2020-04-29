using Autofac;
using Autofac.Extras.AggregateService;

namespace Newbe.Claptrap.Preview.Orleans
{
    public class ClaptrapOrleansModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapTypeCodeFactory>()
                .As<IClaptrapTypeCodeFactory>()
                .SingleInstance();
            builder.RegisterAggregateService<IClaptrapGrainCommonService>();
        }
    }
}