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

        public string GetActorTypeCode(IClaptrapGrain claptrapGrain)
        {
            var claptrapStateAttribute = claptrapGrain.GetType().GetCustomAttribute<ClaptrapStateAttribute>();
            var stateDataType = claptrapStateAttribute.StateDataType;
            var typeCode = _stateDataTypeRegister.FindActorTypeCode(stateDataType);
            return typeCode;
        }
    }
}