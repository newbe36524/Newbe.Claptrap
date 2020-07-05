using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public class RabbitMQEventCenter : IEventCenter
    {
        private readonly IMQSenderManager _senderManager;
        private readonly Lazy<bool> _inited;

        public RabbitMQEventCenter(
            IMQSenderManager senderManager)
        {
            _senderManager = senderManager;
            _inited = new Lazy<bool>(() =>
            {
                senderManager.StartAsync().Wait();
                return true;
            });
        }

        public Task SendToMinionsAsync(IClaptrapIdentity masterId, IEvent @event)
        {
            _ = _inited.Value;
            var sender = _senderManager.Get(@event);
            return sender.SendTopicAsync(@event);
        }
    }
}