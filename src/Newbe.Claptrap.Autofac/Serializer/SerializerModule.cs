using Autofac;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac
{
    public class SerializerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<JsonEventDataStringSerializer>()
                .As<IEventDataStringSerializer>()
                .SingleInstance();

            builder.RegisterType<JsonStateDataStringSerializer>()
                .As<IStateDataStringSerializer>()
                .SingleInstance();
        }
    }
}