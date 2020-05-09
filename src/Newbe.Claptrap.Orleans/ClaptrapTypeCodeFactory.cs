using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Box;
using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Orleans
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
            // to find type code from attribute as this method is invoke before claptrap activated
            var claptrapStateAttribute = claptrapBox
                .GetType()
                .GetInterfaces()
                .Select(x => x.GetCustomAttribute<ClaptrapStateAttribute>())
                .Single(x => x != null);
            var typeCode = claptrapStateAttribute.ActorTypeCode;
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