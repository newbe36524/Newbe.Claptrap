using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public class DefaultStateDataFactoryRegistrationKey : IEquatable<DefaultStateDataFactoryRegistrationKey>
    {
        public IActorKind ActorKind { get; }

        public DefaultStateDataFactoryRegistrationKey(
            IActorKind actorKind)
        {
            ActorKind = actorKind;
        }

        public bool Equals(DefaultStateDataFactoryRegistrationKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ActorKind, other.ActorKind);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DefaultStateDataFactoryRegistrationKey) obj);
        }

        public override int GetHashCode()
        {
            return (ActorKind != null ? ActorKind.GetHashCode() : 0);
        }

        public static bool operator ==(DefaultStateDataFactoryRegistrationKey left,
            DefaultStateDataFactoryRegistrationKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DefaultStateDataFactoryRegistrationKey left,
            DefaultStateDataFactoryRegistrationKey right)
        {
            return !Equals(left, right);
        }
    }
}