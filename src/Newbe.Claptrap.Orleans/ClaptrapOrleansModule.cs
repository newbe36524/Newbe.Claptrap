using Autofac;
using Autofac.Extras.AggregateService;

namespace Newbe.Claptrap.Orleans
{
    public class ClaptrapOrleansModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ActorTypeCodeFactory>()
                .As<IActorTypeCodeFactory>()
                .SingleInstance();
            builder.RegisterAggregateService<IClaptrapGrainCommonService>();
        }
    }
}