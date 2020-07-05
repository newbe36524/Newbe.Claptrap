using Autofac;

namespace Newbe.Claptrap.DataSerializer.Json.Modules
{
    public class JsonSerializerModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Json serializer module";

        public string Description { get; } =
            "Module for registering string serializers for event data and state data. Implement by Newtonsoft.JSON";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<JsonEventDataStringSerializer>()
                .As<IEventDataStringSerializer>()
                .SingleInstance();

            builder.RegisterType<JsonStateDataStringSerializer>()
                .As<IStateDataStringSerializer>()
                .SingleInstance();

            builder.RegisterType<JsonEventStringSerializer>()
                .As<IEventStringSerializer>()
                .SingleInstance();
        }
    }
}