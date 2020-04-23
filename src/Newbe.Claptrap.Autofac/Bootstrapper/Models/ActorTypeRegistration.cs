using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public class ActorTypeRegistration
    {
        private sealed class ActorTypeCodeEqualityComparer : IEqualityComparer<ActorTypeRegistration>
        {
            public bool Equals(ActorTypeRegistration x, ActorTypeRegistration y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.ActorTypeCode == y.ActorTypeCode;
            }

            public int GetHashCode(ActorTypeRegistration obj)
            {
                return obj.ActorTypeCode.GetHashCode();
            }
        }

        public static IEqualityComparer<ActorTypeRegistration> ActorTypeCodeComparer { get; } = new ActorTypeCodeEqualityComparer();

        public string ActorTypeCode { get; set; }
        public Type StateInitialFactoryHandlerType { get; set; }
        public Type ActorStateDataType { get; set; }
    }
}