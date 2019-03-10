using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Autofac
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetBaseTypes(Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        internal static IActorKind GetActorKind(Type grainClassType)
        {
            var claptrapComponentAttribute = grainClassType.GetCustomAttribute<ClaptrapComponentAttribute>();
            if (claptrapComponentAttribute != null)
            {
                return new ClaptrapKind(ActorType.Claptrap, claptrapComponentAttribute.Catalog);
            }

            var minion = grainClassType.GetCustomAttribute<MinionComponentAttribute>();
            if (minion != null)
            {
                return new MinionKind(ActorType.Minion, minion.Catalog, minion.MinionCatalog);
            }

            // todo format exception
            throw new ArgumentOutOfRangeException(nameof(grainClassType));
        }

        public static bool IsClaptrapOrMinionGrainImplement(Type type)
            => type.IsClass &&
               !type.IsAbstract &&
               GetBaseTypes(type).Contains(typeof(Grain)) &&
               (type.GetInterface(nameof(IMinionGrain)) != null ||
                type.GetInterface(nameof(IClaptrapGrain)) != null);

        public static bool IsMinionGrainImplement(Type type)
            => type.IsClass &&
               !type.IsAbstract &&
               GetBaseTypes(type).Contains(typeof(Grain)) &&
               type.GetInterface(nameof(IMinionGrain)) != null;
    }
}