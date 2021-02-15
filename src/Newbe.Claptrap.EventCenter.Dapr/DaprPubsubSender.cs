using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.Dapr.Core;

namespace Newbe.Claptrap.EventCenter.Dapr
{
    public class DaprPubsubSender : IDaprPubsubSender
    {
        private readonly IOptions<ClaptrapServerOptions> _options;
        private readonly DaprClient _daprClient;
        private readonly IEventSerializer<EventJsonModel> _eventStringSerializer;

        public DaprPubsubSender(
            IOptions<ClaptrapServerOptions> options,
            DaprClient daprClient,
            IEventSerializer<EventJsonModel> eventStringSerializer)
        {
            _options = options;
            _daprClient = daprClient;
            _eventStringSerializer = eventStringSerializer;
        }

        public async Task SendTopicAsync(IEvent @event)
        {
            var body = _eventStringSerializer.Serialize(@event);
            var daprOptions = _options.Value.Dapr;
            await _daprClient.PublishEventAsync(daprOptions.PubsubName, daprOptions.Topic, body);
        }
    }
}