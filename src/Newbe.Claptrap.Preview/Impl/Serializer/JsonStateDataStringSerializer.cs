using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Abstractions.Serializer;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Preview.Impl.Serializer
{
    public class JsonStateDataStringSerializer : IStateDataStringSerializer
    {
        private readonly IClaptrapDesignStoreAccessor _claptrapDesignStoreAccessor;

        public JsonStateDataStringSerializer(
            IClaptrapDesignStoreAccessor claptrapDesignStoreAccessor)
        {
            _claptrapDesignStoreAccessor = claptrapDesignStoreAccessor;
        }

        public string Serialize(string claptrapTypeCode, IStateData stateData)
        {
            var re = JsonConvert.SerializeObject(stateData);
            return re;
        }

        public IStateData Deserialize(string claptrapTypeCode, string source)
        {
            var stateDataType = _claptrapDesignStoreAccessor.FindStateDataType(claptrapTypeCode);
            var re = (IStateData) JsonConvert.DeserializeObject(source, stateDataType)!;
            return re;
        }
    }
}