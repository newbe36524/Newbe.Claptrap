using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Assemblies;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap
{
    public class ReflectionActorMetadataProvider : IActorMetadataProvider
    {
        private readonly Lazy<IActorMetadataCollection> _lazy;

        public ReflectionActorMetadataProvider(
            IEnumerable<IActorAssemblyProvider> actorAssemblyProviders)
        {
            _lazy = new Lazy<IActorMetadataCollection>(() =>
            {
                var assemblies = actorAssemblyProviders.SelectMany(x => x.GetAssemblies())
                    .ToArray();
                var re = CreateActorMetadataCollection(assemblies);
                return re;
            });
        }

        public IActorMetadataCollection GetActorMetadata()
        {
            return _lazy.Value;
        }

        private static IActorMetadataCollection CreateActorMetadataCollection(IEnumerable<Assembly> assemblies)
        {
            var allTypes = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var claptrapDic = GetClaptrapMetadata(allTypes)
                .ToDictionary(x => x.ClaptrapKind);

            var minionDic = GetMinionMetadata(allTypes, claptrapDic)
                .ToDictionary(x => x.MinionKind);

            foreach (var ky in claptrapDic)
            {
                var claptrapMetadata = ky.Value;
                claptrapMetadata.MinionMetadata = minionDic.Values.Where(x => x.ClaptrapMetadata == claptrapMetadata)
                    .ToArray();
            }

            var re = new ActorMetadataCollection(claptrapDic, minionDic);
            return re;

            IEnumerable<ClaptrapMetadata> GetClaptrapMetadata(IEnumerable<Type> types)
            {
                foreach (var type in types)
                {
                    var claptrapAttribute = type.GetCustomAttribute<ClaptrapAttribute>();
                    if (claptrapAttribute != null)
                    {
                        var claptrapMetadata = new ClaptrapMetadata
                        {
                            ClaptrapKind = new ClaptrapKind(claptrapAttribute.ActorType, claptrapAttribute.Catalog),
                            StateDataType = claptrapAttribute.StateDataType,
                            ClaptrapEventMetadata =
                                GetClaptrapEventMetadata(type.GetMethods()).Distinct(EventTypeComparer),
                            InterfaceType = type,
                        };
                        yield return claptrapMetadata;
                    }
                }

                IEnumerable<ClaptrapEventMetadata> GetClaptrapEventMetadata(IEnumerable<MethodInfo> methodInfos)
                {
                    foreach (var methodInfo in methodInfos)
                    {
                        var claptrapEventMethodAttribute =
                            methodInfo.GetCustomAttribute<ClaptrapEventAttribute>();
                        if (claptrapEventMethodAttribute != null)
                        {
                            var claptrapEventMetadata = new ClaptrapEventMetadata
                            {
                                EventType = claptrapEventMethodAttribute.EventType,
                                EventDataType = claptrapEventMethodAttribute.EventDataType
                            };
                            yield return claptrapEventMetadata;
                        }
                    }
                }
            }

            IEnumerable<MinionMetadata> GetMinionMetadata(IEnumerable<Type> types,
                IReadOnlyDictionary<IClaptrapKind, ClaptrapMetadata> claptraps)
            {
                foreach (var type in types)
                {
                    var minionAttribute = type.GetCustomAttribute<MinionAttribute>();
                    if (minionAttribute != null)
                    {
                        var claptrapMetadata = claptraps[new ClaptrapKind(ActorType.Claptrap, minionAttribute.Catalog)];
                        var eventTypes = GetMinionEventMetadata(type.GetMethods())
                            .ToArray();
                        var minionMetadata = new MinionMetadata
                        {
                            InterfaceType = type,
                            MinionKind = new MinionKind(minionAttribute.ActorType, minionAttribute.Catalog,
                                minionAttribute.MinionCatalog),
                            StateDataType = minionAttribute.StateDataType,
                            ClaptrapMetadata = claptrapMetadata,
                            ClaptrapEventMetadata =
                                claptrapMetadata.ClaptrapEventMetadata.Where(x => eventTypes.Contains(x.EventType)),
                        };
                        yield return minionMetadata;
                    }
                }

                IEnumerable<string> GetMinionEventMetadata(IEnumerable<MethodInfo> infos)
                {
                    foreach (var methodInfo in infos)
                    {
                        var claptrapEventMethodAttribute =
                            methodInfo.GetCustomAttribute<MinionEventAttribute>();
                        if (claptrapEventMethodAttribute != null)
                        {
                            yield return claptrapEventMethodAttribute.EventType;
                        }
                    }
                }
            }
        }

        private sealed class EventTypeEqualityComparer : IEqualityComparer<ClaptrapEventMetadata>
        {
            public bool Equals(ClaptrapEventMetadata x, ClaptrapEventMetadata y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.EventType, y.EventType);
            }

            public int GetHashCode(ClaptrapEventMetadata obj)
            {
                return (obj.EventType != null ? obj.EventType.GetHashCode() : 0);
            }
        }

        private static IEqualityComparer<ClaptrapEventMetadata> EventTypeComparer { get; } =
            new EventTypeEqualityComparer();

        private class ActorMetadataCollection : IActorMetadataCollection
        {
            private readonly IReadOnlyDictionary<IClaptrapKind, ClaptrapMetadata> _claptrap;
            private readonly IReadOnlyDictionary<IMinionKind, MinionMetadata> _minion;

            public ActorMetadataCollection(
                IReadOnlyDictionary<IClaptrapKind, ClaptrapMetadata> claptrap,
                IReadOnlyDictionary<IMinionKind, MinionMetadata> minion)
            {
                _claptrap = claptrap;
                _minion = minion;
            }

            public ClaptrapMetadata this[IClaptrapKind claptrapKind]
            {
                get
                {
                    if (!_claptrap.TryGetValue(claptrapKind, out var value))
                    {
                        throw new ActorMetadataNotFoundException(claptrapKind);
                    }

                    return value;
                }
            }

            public IEnumerable<ClaptrapMetadata> ClaptrapMetadata => _claptrap.Values;

            public MinionMetadata this[IMinionKind minionKind]
            {
                get
                {
                    if (!_minion.TryGetValue(minionKind, out var value))
                    {
                        throw new ActorMetadataNotFoundException(minionKind);
                    }

                    return value;
                }
            }

            public IEnumerable<MinionMetadata> MinionMetadata => _minion.Values;
        }
    }
}