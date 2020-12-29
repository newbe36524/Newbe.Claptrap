using System.Reflection;
using Dapr.Actors;
using Dapr.Actors.Client;

namespace Newbe.Claptrap.Dapr
{
    public static class ActorProxyFactoryExtensions
    {
        public static T GetClaptrap<T>(this IActorProxyFactory factory, IClaptrapIdentity identity)
            where T : IActor
        {
            var re = factory.CreateActorProxy<T>(new ActorId(identity.Id), identity.TypeCode);
            return re;
        }

        public static T GetClaptrap<T>(this IActorProxyFactory factory, string id)
            where T : IActor
        {
            var attribute = typeof(T).GetCustomAttribute<ClaptrapStateAttribute>()!;
            var re = factory.CreateActorProxy<T>(new ActorId(id), attribute.ClaptrapTypeCode);
            return re;
        }
    }
}