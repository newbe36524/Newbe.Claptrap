using System.Text.Json;

namespace Newbe.Claptrap.DataSerializer.TextJson
{
    public class JsonStateDataStringSerializer : IStateDataStringSerializer
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false
        };

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
            var re = JsonSerializer.Serialize(stateData, stateDataType, JsonSerializerOptions);
            return re;
        }

        public IStateData Deserialize(IClaptrapIdentity identity, string source)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var stateDataType = design.StateDataType;
            var re = (IStateData) JsonSerializer.Deserialize(source, stateDataType, JsonSerializerOptions)!;
            return re;
        }
    }
}