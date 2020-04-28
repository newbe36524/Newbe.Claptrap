using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.Metadata;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Autofac
{
    public class JsonEventDataStringSerializer : IEventDataStringSerializer
    {
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public JsonEventDataStringSerializer(
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public string Serialize(string actorTypeCode, string eventTypeCode, IEventData eventData)
        {
            var re = JsonConvert.SerializeObject(eventData);
            return re;
        }

        public IEventData Deserialize(string actorTypeCode, string eventTypeCode, string source)
        {
            var eventDataType = _claptrapRegistrationAccessor.FindEventDataType(actorTypeCode, eventTypeCode);
            var re = (IEventData) JsonConvert.DeserializeObject(source, eventDataType)!;
            return re;
        }
    }
}