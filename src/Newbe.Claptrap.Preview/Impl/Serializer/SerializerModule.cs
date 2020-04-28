using Autofac;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
{
    public class SerializerModule : Autofac.Module
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