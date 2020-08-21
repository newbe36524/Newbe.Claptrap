using System;
using Newtonsoft.Json;

namespace Newbe.Claptrap.Saga
{
    public class SagaUserDataSerializer : ISagaUserDataSerializer
    {
        public string Serialize(object userData, Type userDataType)
        {
            var re = JsonConvert.SerializeObject(userData, userDataType, null);
            return re;
        }

        public object Deserialize(string source, Type userDataType)
        {
            var re = JsonConvert.DeserializeObject(source, userDataType);
            return re!;
        }
    }
}