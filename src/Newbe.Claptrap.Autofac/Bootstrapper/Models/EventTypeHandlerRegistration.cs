using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public class EventTypeHandlerRegistration
    {
        public string EventTypeCode { get; set; }
        public string ActorTypeCode { get; set; }
        public Type EventDataType { get; set; }
        public Type EventHandlerType { get; set; }

        private sealed class EventTypeCodeActorTypeCodeEqualityComparer : IEqualityComparer<EventTypeHandlerRegistration>
        {
            public bool Equals(EventTypeHandlerRegistration x, EventTypeHandlerRegistration y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.EventTypeCode == y.EventTypeCode && x.ActorTypeCode == y.ActorTypeCode;
            }

            public int GetHashCode(EventTypeHandlerRegistration obj)
            {
                return HashCode.Combine(obj.EventTypeCode, obj.ActorTypeCode);
            }
        }

        public static IEqualityComparer<EventTypeHandlerRegistration> EventTypeCodeActorTypeCodeComparer { get; } = new EventTypeCodeActorTypeCodeEqualityComparer();
    }
}