using Newtonsoft.Json;

namespace Newbe.Claptrap.DataSerializer.JsonNet
{
    public class JsonStateDataStringSerializer : IStateDataStringSerializer
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public JsonStateDataStringSerializer(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public string Serialize(IClaptrapIdentity identity, IStateData stateData)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var stateDataType = design.StateDataType;
            var re = JsonConvert.SerializeObject(stateData, stateDataType, new JsonSerializerSettings
            {
                Formatting = Formatting.None
            });
            return re;
        }

        public IStateData Deserialize(IClaptrapIdentity identity, string source)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var stateDataType = design.StateDataType;
            var re = (IStateData) JsonConvert.DeserializeObject(source, stateDataType)!;
            return re;
        }
    }
}