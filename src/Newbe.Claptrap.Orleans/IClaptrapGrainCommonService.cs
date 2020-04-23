using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapGrainCommonService
    {
        IActorFactory ActorFactory { get; }
        IActorTypeCodeFactory ActorTypeCodeFactory { get; }
    }
}