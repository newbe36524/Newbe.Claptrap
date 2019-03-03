using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public class StateDataUpdaterRegistrationKey : IEquatable<StateDataUpdaterRegistrationKey>
    {
        public IActorKind ActorKind { get; }
        public string EventType { get; }

        public StateDataUpdaterRegistrationKey(
            IActorKind actorKind,
            string eventType)
        {
            ActorKind = actorKind;
            EventType = eventType;
        }

        public bool Equals(StateDataUpdaterRegistrationKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ActorKind, other.ActorKind) && string.Equals(EventType, other.EventType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StateDataUpdaterRegistrationKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ActorKind != null ? ActorKind.GetHashCode() : 0) * 397) ^
                       (EventType != null ? EventType.GetHashCode() : 0);
            }
        }

        public static bool operator ==(StateDataUpdaterRegistrationKey left, StateDataUpdaterRegistrationKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateDataUpdaterRegistrationKey left, StateDataUpdaterRegistrationKey right)
        {
            return !Equals(left, right);
        }
    }
}