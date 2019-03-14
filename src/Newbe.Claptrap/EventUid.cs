using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public class EventUid : IEventUid
    {
        public EventUid(string uid)
        {
            Uid = uid;
        }

        public string Uid { get; }

        public bool Equals(IEventUid other)
        {
            if (other is EventUid uid)
            {
                return string.Equals(Uid, uid.Uid);
            }

            return false;
        }

        protected bool Equals(EventUid other)
        {
            return string.Equals(Uid, other.Uid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventUid) obj);
        }

        public override int GetHashCode()
        {
            return (Uid != null ? Uid.GetHashCode() : 0);
        }
    }
}