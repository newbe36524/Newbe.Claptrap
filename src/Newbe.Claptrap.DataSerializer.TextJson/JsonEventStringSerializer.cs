using System.Text.Json;
using System.Text.Json.Serialization;

namespace Newbe.Claptrap.DataSerializer.TextJson
{
    public class JsonEventStringSerializer : IEventStringSerializer
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false
        };

        private readonly IEventDataStringSerializer _eventDataStringSerializer;

        public JsonEventStringSerializer(
            IEventDataStringSerializer eventDataStringSerializer)
        {
            _eventDataStringSerializer = eventDataStringSerializer;
        }

        public string Serialize(IEvent evt)
        {
            var id = evt.ClaptrapIdentity;
            var eventData = _eventDataStringSerializer.Serialize(id, evt.EventTypeCode, evt.Data);
            var model = new EventJsonModel
            {
                Version = evt.Version,
                ClaptrapId = id.Id,
                ClaptrapTypeCode = id.TypeCode,
                EventTypeCode = evt.EventTypeCode,
                DataJson = eventData
            };
            var result = JsonSerializer.Serialize(model, JsonSerializerOptions);
            return result;
        }

        public IEvent Deserialize(string source)
        {
            var jsonModel = JsonSerializer.Deserialize<EventJsonModel>(source, JsonSerializerOptions)!;
            var id = new ClaptrapIdentity(jsonModel.ClaptrapId, jsonModel.ClaptrapTypeCode);
            var eventData = _eventDataStringSerializer.Deserialize(
                id,
                jsonModel.EventTypeCode,
                jsonModel.DataJson);
            var re = new DataEvent(id, jsonModel.EventTypeCode, eventData)
            {
                Version = jsonModel.Version
            };
            return re;
        }
    }

    public class EventJsonModel
    {
        [JsonPropertyName("ctc")] public string ClaptrapTypeCode { get; set; }

        [JsonPropertyName("cid")] public string ClaptrapId { get; set; }

        [JsonPropertyName("v")] public long Version { get; set; }

        [JsonPropertyName("etc")] public string EventTypeCode { get; set; }

        [JsonPropertyName("d")] public string DataJson { get; set; }
    }
}