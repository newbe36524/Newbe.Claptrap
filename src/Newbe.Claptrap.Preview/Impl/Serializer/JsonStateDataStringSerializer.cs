using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Abstractions.Serializer;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Preview.Impl.Serializer
{
    public class JsonStateDataStringSerializer : IStateDataStringSerializer
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public JsonStateDataStringSerializer(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public string Serialize(string claptrapTypeCode, IStateData stateData)
        {
            var re = JsonConvert.SerializeObject(stateData);
            return re;
        }

        public IStateData Deserialize(string claptrapTypeCode, string source)
        {
            var design = _claptrapDesignStore.FindDesign(new ClaptrapIdentity(default!, claptrapTypeCode));
            var stateDataType = design.StateDataType;
            var re = (IStateData) JsonConvert.DeserializeObject(source, stateDataType)!;
            return re;
        }
    }
}