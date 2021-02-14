using System;
using System.Linq;
using System.Reflection;

namespace Newbe.Claptrap.Dapr
{
    public class ClaptrapTypeCodeFactory : IClaptrapTypeCodeFactory
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapTypeCodeFactory(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public string GetClaptrapTypeCode(IClaptrapBox claptrapBox)
        {
            return GetClaptrapTypeCode(claptrapBox.GetType());
        }

        public string GetClaptrapTypeCode(Type interfaceType)
        {
            // to find type code from attribute as this method is invoke before claptrap activated. Identity is unable to be used.
            var claptrapStateAttribute = interfaceType
                .GetInterfaces()
                .Select(x => x.GetCustomAttribute<ClaptrapStateAttribute>())
                .Single(x => x != null)!;
            var typeCode = claptrapStateAttribute.ClaptrapTypeCode;
            return typeCode;
        }

        public string FindEventTypeCode<TEventDataType>(IClaptrapBox claptrapBox, TEventDataType eventDataType)
        {
            var claptrapDesign = _claptrapDesignStore.FindDesign(claptrapBox.Claptrap.State.Identity);
            var (key, _) =
                claptrapDesign.EventHandlerDesigns.SingleOrDefault(x =>
                    x.Value.EventDataType == typeof(TEventDataType));
            return key;
        }
    }
}