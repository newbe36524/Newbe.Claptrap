using System;
using System.Collections.Generic;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.Autofac
{
    public class EventStoreRegistration
    {
        public string ActorTypeCode { get; set; }
        public EventStoreProvider EventStoreProvider { get; set; }

        private sealed class ActorTypeCodeEqualityComparer : IEqualityComparer<EventStoreRegistration>
        {
            public bool Equals(EventStoreRegistration x, EventStoreRegistration y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.ActorTypeCode == y.ActorTypeCode;
            }

            public int GetHashCode(EventStoreRegistration obj)
            {
                return obj.ActorTypeCode.GetHashCode();
            }
        }

        public static IEqualityComparer<EventStoreRegistration> ActorTypeCodeComparer { get; } = new ActorTypeCodeEqualityComparer();
    }
}