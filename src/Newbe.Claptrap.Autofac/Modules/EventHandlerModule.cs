using Autofac;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventHandlers;

namespace Newbe.Claptrap.Autofac.Modules
{
    public class EventHandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapStateEventHandler>()
                .AsSelf()
                .PerEventScope();
            builder.RegisterType<ClaptrapEventPublishEventHandler>()
                .AsSelf()
                .PerEventScope();
            builder.RegisterType<MinionEventHandler>()
                .AsSelf()
                .PerEventScope();
            builder.RegisterType<StateRestoreEventHandler>()
                .AsSelf()
                .PerEventScope();
            builder.RegisterType<AutofacEventLifetimeScope>()
                .AsSelf()
                .As<IEventLifetimeScope>()
                .PerEventScope();
            builder.Register(context =>
                    context.Resolve<IEventLifetimeScope>().EventContext)
                .As<IEventContext>()
                .PerEventScope();
            builder.RegisterType<AutofacEventHandlerFactory>()
                .As<IEventHandlerFactory>();
            builder.RegisterType<AutofacMinionEventHandlerFactory>()
                .As<IMinionEventHandlerFactory>();
            builder.RegisterType<NoneStateDataStateDataUpdater>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<NoneStateDataStateDataFactory>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<AutofacStateDataUpdaterFactory>()
                .As<IStateDataUpdaterFactory>();
        }
    }
}