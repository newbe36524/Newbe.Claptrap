using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Attributes;

namespace Newbe.Claptrap.Preview.Orleans
{
    public class ClaptrapTypeCodeFactory : IClaptrapTypeCodeFactory
    {
        private readonly IClaptrapDesignStoreAccessor _claptrapDesignStoreAccessor;

        public ClaptrapTypeCodeFactory(
            IClaptrapDesignStoreAccessor claptrapDesignStoreAccessor)
        {
            _claptrapDesignStoreAccessor = claptrapDesignStoreAccessor;
        }

        public string GetClaptrapTypeCode(IClaptrapBox claptrapBox)
        {
            var claptrapStateAttribute = claptrapBox
                .GetType()
                .GetInterfaces()
                .Select(x => x.GetCustomAttribute<ClaptrapStateAttribute>())
                .Single(x => x != null);
            var stateDataType = claptrapStateAttribute.StateDataType;
            var typeCode = _claptrapDesignStoreAccessor.FindActorTypeCode(stateDataType);
            return typeCode;
        }
    }
}