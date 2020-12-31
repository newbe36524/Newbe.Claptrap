using Newtonsoft.Json;

namespace Newbe.Claptrap.DataSerializer.JsonNet
{
    public class JsonEventStringSerializer : IEventStringSerializer
    {
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
            var result = JsonConvert.SerializeObject(model);
            return result;
        }

        public IEvent Deserialize(string source)
        {
            var jsonModel = JsonConvert.DeserializeObject<EventJsonModel>(source);
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
        [JsonProperty("ctc")]
        public string ClaptrapTypeCode { get; set; }

        [JsonProperty("cid")]
        public string ClaptrapId { get; set; }

        [JsonProperty("v")]
        public long Version { get; set; }

        [JsonProperty("etc")]
        public string EventTypeCode { get; set; }

        [JsonProperty("d")]
        public string DataJson { get; set; }
    }
}