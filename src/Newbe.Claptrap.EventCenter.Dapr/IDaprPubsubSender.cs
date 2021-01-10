using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter.Dapr
{
    public interface IDaprPubsubSender
    {
        Task SendTopicAsync(IEvent @event);
    }
}