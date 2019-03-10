using System.Collections.Generic;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventChannels;

namespace Newbe.Claptrap.Autofac
{
    public interface IEventPublishChannelFactory
    {
        IEnumerable<IEventPublishChannel> Create(IActorIdentity identity);
    }
}