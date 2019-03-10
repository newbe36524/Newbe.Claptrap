using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public class MinionEventHandlerRegistrationKey : IEquatable<MinionEventHandlerRegistrationKey>
    {
        public IMinionKind MinionKind { get; }
        public string EventType { get; }

        public MinionEventHandlerRegistrationKey(IMinionKind minionKind, string eventType)
        {
            MinionKind = minionKind;
            EventType = eventType;
        }

        public bool Equals(MinionEventHandlerRegistrationKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(MinionKind, other.MinionKind) && string.Equals(EventType, other.EventType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MinionEventHandlerRegistrationKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((MinionKind != null ? MinionKind.GetHashCode() : 0) * 397) ^
                       (EventType != null ? EventType.GetHashCode() : 0);
            }
        }
    }
}