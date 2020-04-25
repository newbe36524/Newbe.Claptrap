using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Orleans
{
    public class ActorTypeCodeFactory : IActorTypeCodeFactory
    {
        private readonly IStateDataTypeRegister _stateDataTypeRegister;

        public ActorTypeCodeFactory(
            IStateDataTypeRegister stateDataTypeRegister)
        {
            _stateDataTypeRegister = stateDataTypeRegister;
        }

        public string GetActorTypeCode(IClaptrap claptrap)
        {
            var claptrapStateAttribute = claptrap
                .GetType()
                .GetInterfaces()
                .Select(x => x.GetCustomAttribute<ClaptrapStateAttribute>())
                .Single(x => x != null);
            var stateDataType = claptrapStateAttribute.StateDataType;
            var typeCode = _stateDataTypeRegister.FindActorTypeCode(stateDataType);
            return typeCode;
        }
    }
}