using System.Linq;
using Newtonsoft.Json;

namespace Newbe.Claptrap.DataSerializer.JsonNet
{
    public class JsonEventDataStringSerializer : IEventDataStringSerializer
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public JsonEventDataStringSerializer(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public string Serialize(IClaptrapIdentity identity, string eventTypeCode, IEventData eventData)
        {
            var re = JsonConvert.SerializeObject(eventData);
            return re;
        }

        public IEventData Deserialize(IClaptrapIdentity identity, string eventTypeCode, string source)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var (_, value) = design.EventHandlerDesigns.Single(x => x.Key == eventTypeCode);
            var re = (IEventData) JsonConvert.DeserializeObject(source, value.EventDataType)!;
            return re;
        }
    }
}