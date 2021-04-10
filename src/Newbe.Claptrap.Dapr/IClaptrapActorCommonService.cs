using Autofac;
using Autofac.Core.Lifetime;
using Dapr.Actors.Runtime;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.Dapr
{
    public interface IClaptrapActorCommonService
    {
        IClaptrapFactory ClaptrapFactory { get; }
        IClaptrapAccessor ClaptrapAccessor { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
        IEventSerializer<EventJsonModel> EventSerializer { get; }
        ILifetimeScope LifetimeScope { get; }
    }
}