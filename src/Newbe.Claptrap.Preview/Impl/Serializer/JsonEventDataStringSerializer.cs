using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Abstractions.Serializer;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Preview.Impl.Serializer
{
    public class JsonEventDataStringSerializer : IEventDataStringSerializer
    {
        private readonly IClaptrapDesignStoreAccessor _claptrapDesignStoreAccessor;

        public JsonEventDataStringSerializer(
            IClaptrapDesignStoreAccessor claptrapDesignStoreAccessor)
        {
            _claptrapDesignStoreAccessor = claptrapDesignStoreAccessor;
        }

        public string Serialize(string claptrapTypeCode, string eventTypeCode, IEventData eventData)
        {
            var re = JsonConvert.SerializeObject(eventData);
            return re;
        }

        public IEventData Deserialize(string claptrapTypeCode, string eventTypeCode, string source)
        {
            var eventDataType = _claptrapDesignStoreAccessor.FindEventDataType(claptrapTypeCode, eventTypeCode);
            var re = (IEventData) JsonConvert.DeserializeObject(source, eventDataType)!;
            return re;
        }
    }
}