using System.Text.Json;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.DataSerializer.TextJson
{
    public class JsonEventStringSerializer : IEventStringSerializer, IEventSerializer<EventJsonModel>
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
            var model = (this as IEventSerializer<EventJsonModel>).Serialize(evt);
            var result = JsonSerializer.Serialize(model, JsonSerializerOptions);
            return result;
        }

        public IEvent Deserialize(EventJsonModel source)
        {
            var id = new ClaptrapIdentity(source.cid, source.ctc);
            var eventData = _eventDataStringSerializer.Deserialize(
                id,
                source.etc,
                source.d);
            var re = new DataEvent(id, source.etc, eventData)
            {
                Version = source.v
            };
            return re;
        }

        public IEvent Deserialize(string source)
        {
            var jsonModel = JsonSerializer.Deserialize<EventJsonModel>(source, JsonSerializerOptions)!;
            return Deserialize(jsonModel);
        }

        EventJsonModel IEventSerializer<EventJsonModel>.Serialize(IEvent evt)
        {
            var id = evt.ClaptrapIdentity;
            var eventData = _eventDataStringSerializer.Serialize(id, evt.EventTypeCode, evt.Data);
            var model = new EventJsonModel
            {
                v = evt.Version,
                cid = id.Id,
                ctc = id.TypeCode,
                etc = evt.EventTypeCode,
                d = eventData
            };
            return model;
        }
    }
}