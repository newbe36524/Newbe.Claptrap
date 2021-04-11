using Autofac;
using HelloClaptrap.Actors.AuctionItem;

namespace HelloClaptrap.Actors
{
    public class ActorsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<AuctionItemActorStateLoader>()
                .AsSelf();
        }
    }
}