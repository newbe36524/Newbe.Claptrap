using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapGrainCommonService
    {
        IActorFactory ActorFactory { get; }
        IActorTypeCodeFactory ActorTypeCodeFactory { get; }
    }
}