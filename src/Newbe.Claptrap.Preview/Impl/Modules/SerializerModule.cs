using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Serializer;
using Newbe.Claptrap.Preview.Impl.Serializer;

namespace Newbe.Claptrap.Preview.Impl.Modules
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