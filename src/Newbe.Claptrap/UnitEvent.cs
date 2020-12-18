using System;
using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap
{
    /// <summary>
    /// This event is used for load test
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record UnitEvent : IEvent
    {
        public UnitEvent()
        {
        }

        public UnitEvent(IClaptrapIdentity claptrapIdentity, string eventTypeCode, IEventData data)
        {
            ClaptrapIdentity = claptrapIdentity;
            EventTypeCode = eventTypeCode;
            Data = data;
        }

        public const string TypeCode = "unit_event";

        public IClaptrapIdentity ClaptrapIdentity { get; } = null!;
        public long Version { get; set; }
        public string EventTypeCode { get; } = TypeCode;
        public IEventData Data { get; } = null!;

        public class UnitEventData : IEventData
        {
            public string Item1 { get; set; } = null!;
            public string Item2 { get; set; } = null!;
            public string Item3 { get; set; } = null!;
            public string Item4 { get; set; } = null!;
            public string Item5 { get; set; } = null!;

            public static UnitEventData Create()
            {
                return new UnitEventData
                {
                    Item1 = Guid.NewGuid().ToString("N"),
                    Item2 = Guid.NewGuid().ToString("N"),
                    Item3 = Guid.NewGuid().ToString("N"),
                    Item4 = Guid.NewGuid().ToString("N"),
                    Item5 = Guid.NewGuid().ToString("N")
                };
            }
        }

        public static UnitEvent Create(IClaptrapIdentity identity)
        {
            return new UnitEvent(identity, TypeCode, UnitEventData.Create());
        }
    }
}