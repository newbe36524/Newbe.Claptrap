using System;
using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class StateEntity : IStateEntity
    {
        public string ClaptrapTypeCode { get; set; } = null!;
        public string ClaptrapId { get; set; } = null!;
        public long Version { get; set; }
        public string StateData { get; set; } = null!;
        public DateTime UpdatedTime { get; set; }

        private sealed class ClaptrapTypeCodeClaptrapIdEqualityComparer : IEqualityComparer<StateEntity>
        {
            public bool Equals(StateEntity x, StateEntity y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.ClaptrapTypeCode == y.ClaptrapTypeCode && x.ClaptrapId == y.ClaptrapId;
            }

            public int GetHashCode(StateEntity obj)
            {
                return HashCode.Combine(obj.ClaptrapTypeCode, obj.ClaptrapId);
            }
        }

        public static IEqualityComparer<StateEntity> ClaptrapTypeCodeClaptrapIdComparer { get; } =
            new ClaptrapTypeCodeClaptrapIdEqualityComparer();

        public static IEnumerable<StateEntity> DistinctWithVersion(IEnumerable<StateEntity> entities)
        {
            var array = entities as StateEntity[] ?? entities.ToArray();
            var set = new HashSet<StateEntity>(array.Length, ClaptrapTypeCodeClaptrapIdComparer);
            foreach (var stateEntity in array.OrderBy(x => x.Version))
            {
                if (set.TryGetValue(stateEntity, out var old))
                {
                    if (old.Version < stateEntity.Version)
                    {
                        set.Remove(old);
                        set.Add(stateEntity);
                    }
                }
                else
                {
                    set.Add(stateEntity);
                }
            }

            return set;
        }
    }
}