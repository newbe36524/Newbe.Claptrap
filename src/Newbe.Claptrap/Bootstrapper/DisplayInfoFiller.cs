using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class DisplayInfoFiller
    {
        public const string ClaptrapDisplayName = "ClaptrapDisplayName";
        public const string EventDisplayName = "EventDisplayName";
        public const string EventHandlerDisplayName = "EventHandlerDisplayName";

        public const string ClaptrapDescription = "ClaptrapDescription";
        public const string EventDescription = "EventDescription";
        public const string EventHandlerDescription = "EventHandlerDescription";

        public static void FillDisplayInfo(IClaptrapDesign design)
        {
            SetValueIfFound(design.ExtendInfos, ClaptrapDisplayName, GetDisplayName(new[]
            {
                design.ClaptrapBoxImplementationType,
                design.ClaptrapBoxInterfaceType,
                design.StateDataType
            }));

            SetValueIfFound(design.ExtendInfos, ClaptrapDescription, GetDescription(new[]
            {
                design.ClaptrapBoxImplementationType,
                design.ClaptrapBoxInterfaceType,
                design.StateDataType
            }));

            foreach (var (_, eventHandlerDesign) in design.EventHandlerDesigns)
            {
                SetValueIfFound(eventHandlerDesign.ExtendInfos, EventDisplayName, GetDisplayName(new[]
                {
                    eventHandlerDesign.EventDataType,
                }));

                SetValueIfFound(eventHandlerDesign.ExtendInfos, EventDescription, GetDescription(new[]
                {
                    eventHandlerDesign.EventDataType,
                }));

                SetValueIfFound(eventHandlerDesign.ExtendInfos, EventHandlerDisplayName, GetDisplayName(new[]
                {
                    eventHandlerDesign.EventHandlerType,
                }));

                SetValueIfFound(eventHandlerDesign.ExtendInfos, EventHandlerDescription, GetDescription(new[]
                {
                    eventHandlerDesign.EventHandlerType,
                }));
            }

            static void SetValueIfFound(IDictionary<string, object> extDic, string key, IEnumerable<string> values)
            {
                var value = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    extDic[key] = value;
                }
            }
        }

        private static IEnumerable<string> GetDisplayName(
            IEnumerable<MemberInfo?> types)
        {
            return types.Where(type => type != null)
                .Select(GetDisplayName!)
                .Where(displayName => !string.IsNullOrEmpty(displayName))!;
        }

        private static string? GetDisplayName(MemberInfo type)
        {
            return type
                ?.GetCustomAttribute<ClaptrapDisplayNameAttribute>()
                ?.DisplayName;
        }

        private static IEnumerable<string> GetDescription(
            IEnumerable<MemberInfo?> types)
        {
            return types.Where(type => type != null)
                .Select(GetDescription!)
                .Where(descrption => !string.IsNullOrEmpty(descrption))!;
        }

        private static string? GetDescription(MemberInfo type)
        {
            return type
                ?.GetCustomAttribute<ClaptrapDescriptionAttribute>()
                ?.Description;
        }
    }
}