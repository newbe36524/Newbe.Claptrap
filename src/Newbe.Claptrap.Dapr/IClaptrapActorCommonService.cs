using Autofac;
using Dapr.Actors.Runtime;

namespace Newbe.Claptrap.Dapr
{
    public interface IClaptrapActorCommonService
    {
        IClaptrapFactory ClaptrapFactory { get; }
        IClaptrapAccessor ClaptrapAccessor { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
        ActorHost ActorHost { get; }
        ILifetimeScope LifetimeScope { get; }
    }
}