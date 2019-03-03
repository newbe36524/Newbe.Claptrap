using System.Collections.Generic;
using Autofac;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;

namespace Newbe.Claptrap.Autofac.Modules
{
    public class EventPublishChannelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EventPublishChannelFactory>()
                .As<IEventPublishChannelFactory>();
            builder.Register(context =>
                    context.Resolve<IEventPublishChannelFactory>().Create(context.Resolve<IActorIdentity>()))
                .As<IEnumerable<IEventPublishChannel>>()
                .PerActorScope();
        }
    }
}