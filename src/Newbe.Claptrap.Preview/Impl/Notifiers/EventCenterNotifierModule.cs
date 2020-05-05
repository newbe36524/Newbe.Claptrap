using System;
using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EventCenterNotifierModule : Module
    {
        private readonly IClaptrapIdentity _identity;

        public EventCenterNotifierModule(
            IClaptrapIdentity identity)
        {
            _identity = identity;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EventCenterEventHandledNotifier>()
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterType<EventCenterEventHandledNotifierFactory>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<MemoryOrleansEventCenter>()
                .As<IEventCenter>()
                .SingleInstance();
            RegisterComponent<IEventHandledNotifier>(typeof(EventCenterEventHandledNotifierFactory));

            void RegisterComponent<TComponent>(Type factoryType)
                where TComponent : class, IClaptrapComponent
            {
                builder.Register(t =>
                        ((IClaptrapComponentFactory<TComponent>) t.Resolve(factoryType))
                        .Create(_identity))
                    .As<TComponent>()
                    .SingleInstance();
            }
        }
    }
}