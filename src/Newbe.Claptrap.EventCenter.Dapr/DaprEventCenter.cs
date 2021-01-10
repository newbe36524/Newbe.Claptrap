using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter.Dapr
{
    public class DaprEventCenter : IEventCenter
    {
        private readonly IDaprPubsubSender _daprPubsubSender;

        public DaprEventCenter(
            IDaprPubsubSender daprPubsubSender)
        {
            _daprPubsubSender = daprPubsubSender;
        }

        public Task SendToMinionsAsync(IClaptrapIdentity masterId, IEvent @event)
        {
            return _daprPubsubSender.SendTopicAsync(@event);
        }
    }
}