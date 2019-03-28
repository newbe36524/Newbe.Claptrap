using Autofac;
using Newbe.Claptrap.EventChannels;

namespace Newbe.Claptrap.EventHub.DirectClient
{
    public class DirectClientEventHubModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<DirectClientEventPublishChannelProvider>()
                .As<IEventPublishChannelProvider>();
        }
    }
}