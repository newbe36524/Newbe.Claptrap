using System.Linq;
using Newtonsoft.Json;

namespace Newbe.Claptrap.DataSerializer.Json
{
    public class JsonEventDataStringSerializer : IEventDataStringSerializer
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public JsonEventDataStringSerializer(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public string Serialize(string claptrapTypeCode, string eventTypeCode, IEventData eventData)
        {
            var re = JsonConvert.SerializeObject(eventData);
            return re;
        }

        public IEventData Deserialize(string claptrapTypeCode, string eventTypeCode, string source)
        {
            var design = _claptrapDesignStore.FindDesign(new ClaptrapIdentity(default!, claptrapTypeCode));
            var (_, value) = design.EventHandlerDesigns.Single(x => x.Key == eventTypeCode);
            var re = (IEventData) JsonConvert.DeserializeObject(source, value.EventDataType)!;
            return re;
        }
    }
}