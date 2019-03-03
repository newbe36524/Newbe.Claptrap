using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventChannels
{
    public interface IEventPublishChannelProvider
    {
        IEventPublishChannel Create(IActorIdentity claptrapIdentity, IMinionKind minionKind);
    }
}