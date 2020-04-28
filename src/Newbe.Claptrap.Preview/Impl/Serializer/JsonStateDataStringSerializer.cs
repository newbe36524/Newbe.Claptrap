using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.Metadata;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Preview
{
    public class JsonStateDataStringSerializer : IStateDataStringSerializer
    {
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public JsonStateDataStringSerializer(
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public string Serialize(string actorTypeCode, IStateData stateData)
        {
            var re = JsonConvert.SerializeObject(stateData);
            return re;
        }

        public IStateData Deserialize(string actorTypeCode, string source)
        {
            var stateDataType = _claptrapRegistrationAccessor.FindStateDataType(actorTypeCode);
            var re = (IStateData) JsonConvert.DeserializeObject(source, stateDataType)!;
            return re;
        }
    }
}