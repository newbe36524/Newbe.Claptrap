using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Preview.Metadata;

namespace Newbe.Claptrap.Preview.Orleans
{
    public class ActorTypeCodeFactory : IActorTypeCodeFactory
    {
        private readonly IClaptrapRegistrationAccessor _claptrapRegistrationAccessor;

        public ActorTypeCodeFactory(
            IClaptrapRegistrationAccessor claptrapRegistrationAccessor)
        {
            _claptrapRegistrationAccessor = claptrapRegistrationAccessor;
        }

        public string GetActorTypeCode(IClaptrap claptrap)
        {
            var claptrapStateAttribute = claptrap
                .GetType()
                .GetInterfaces()
                .Select(x => x.GetCustomAttribute<ClaptrapStateAttribute>())
                .Single(x => x != null);
            var stateDataType = claptrapStateAttribute.StateDataType;
            var typeCode = _claptrapRegistrationAccessor.FindActorTypeCode(stateDataType);
            return typeCode;
        }
    }
}