using System;
using Autofac;

namespace Newbe.Claptrap.Notifiers
{
    public class EmptyNotifierModule : Module
    {
        private readonly IClaptrapIdentity _identity;

        public EmptyNotifierModule(
            IClaptrapIdentity identity)
        {
            _identity = identity;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EmptyEventNotifier>()
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterType<EmptyEventHandledNotifierFactory>()
                .AsSelf()
                .SingleInstance();
            RegisterComponent<IEventNotifier>(typeof(EmptyEventHandledNotifierFactory));

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