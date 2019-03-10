using Autofac;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac.Modules
{
    public class ActorFactoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<AutofacActorFactory>()
                .As<IActorFactory>();
            builder.RegisterType<Actor>()
                .AsSelf();
        }
    }
}