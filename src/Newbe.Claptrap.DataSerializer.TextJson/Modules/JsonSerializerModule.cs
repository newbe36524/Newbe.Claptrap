using Autofac;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.DataSerializer.TextJson.Modules
{
    public class JsonSerializerModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Json serializer module";

        public string Description { get; } =
            "Module for registering string serializers for event data and state data. Implement by System.Text.Json";

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
                .As<IEventSerializer<EventJsonModel>>()
                .SingleInstance();
        }
    }
}