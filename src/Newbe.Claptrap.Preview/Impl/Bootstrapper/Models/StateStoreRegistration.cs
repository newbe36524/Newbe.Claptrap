using System.Collections.Generic;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview
{
    public class StateStoreRegistration
    {
        public string ActorTypeCode { get; set; }
        public StateStoreProvider StateStoreProvider { get; set; }

        private sealed class ActorTypeCodeEqualityComparer : IEqualityComparer<StateStoreRegistration>
        {
            public bool Equals(StateStoreRegistration x, StateStoreRegistration y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.ActorTypeCode == y.ActorTypeCode;
            }

            public int GetHashCode(StateStoreRegistration obj)
            {
                return obj.ActorTypeCode.GetHashCode();
            }
        }

        public static IEqualityComparer<StateStoreRegistration> ActorTypeCodeComparer { get; } =
            new ActorTypeCodeEqualityComparer();
    }
}