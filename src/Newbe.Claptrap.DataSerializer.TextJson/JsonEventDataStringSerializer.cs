using System.Text.Json;

namespace Newbe.Claptrap.DataSerializer.TextJson
{
    public class JsonEventDataStringSerializer : IEventDataStringSerializer
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false
        };

        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public JsonEventDataStringSerializer(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public string Serialize(IClaptrapIdentity identity, string eventTypeCode, IEventData eventData)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var claptrapEventHandlerDesign = design.EventHandlerDesigns[eventTypeCode];
            var re = JsonSerializer.Serialize(eventData,
                claptrapEventHandlerDesign.EventDataType,
                JsonSerializerOptions);
            return re;
        }

        public IEventData Deserialize(IClaptrapIdentity identity, string eventTypeCode, string source)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var claptrapEventHandlerDesign = design.EventHandlerDesigns[eventTypeCode];
            var re = (IEventData) JsonSerializer.Deserialize(source,
                claptrapEventHandlerDesign.EventDataType,
                JsonSerializerOptions)!;
            return re;
        }
    }
}