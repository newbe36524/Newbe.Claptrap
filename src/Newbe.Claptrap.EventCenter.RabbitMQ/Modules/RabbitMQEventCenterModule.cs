using Autofac;
using Newbe.Claptrap.EventCenter.RabbitMQ.Impl;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Modules
{
    public class RabbitMQEventCenterModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Claptrap RabbitMQ module";
        public string Description { get; } = "Module for Claptrap EventCenter implement by RabbitMQ";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MessageSerializer>()
                .As<IMessageSerializer>()
                .SingleInstance();

            builder.RegisterType<MqSender>()
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterType<MQSenderManager>()
                .As<IMQSenderManager>()
                .ExternallyOwned()
                .SingleInstance();
            builder.RegisterType<MQSubscriberManager>()
                .As<IMQSubscriberManager>()
                .ExternallyOwned()
                .SingleInstance();
            builder.RegisterType<ConnectionManager>()
                .As<IConnectionManager>()
                .ExternallyOwned()
                .SingleInstance();
        }
    }
}