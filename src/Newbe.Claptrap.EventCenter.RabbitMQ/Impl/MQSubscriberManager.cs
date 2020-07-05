using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class MQSubscriberManager : IMQSubscriberManager
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IEventStringSerializer _eventStringSerializer;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMinionLocator _minionLocator;
        private IReadOnlyDictionary<string, IModel>? _consumers;

        public MQSubscriberManager(
            IConnectionManager connectionManager,
            IEventStringSerializer eventStringSerializer,
            IClaptrapDesignStore claptrapDesignStore,
            IMessageSerializer messageSerializer,
            IMinionLocator minionLocator)
        {
            _connectionManager = connectionManager;
            _eventStringSerializer = eventStringSerializer;
            _claptrapDesignStore = claptrapDesignStore;
            _messageSerializer = messageSerializer;
            _minionLocator = minionLocator;
        }

        public Task StartAsync()
        {
            return Task.Run(StartCore);

            void StartCore()
            {
                var connection = _connectionManager.CreateConnection();
                _consumers = _claptrapDesignStore
                    .Where(x => x.IsMinion())
                    .ToDictionary(x => x.ClaptrapTypeCode,
                        x =>
                        {
                            var masterDesign = x.ClaptrapMasterDesign;
                            var model = connection.CreateModel();
                            var exchangeName = TopicHelper.GetExchangeName(masterDesign.ClaptrapTypeCode);
                            var subscribeKey = TopicHelper.GetSubscribeKey(masterDesign.ClaptrapTypeCode);
                            var queueName = TopicHelper.GetQueueName(x.ClaptrapTypeCode);
                            model.ExchangeDeclare(exchangeName, ExchangeType.Topic);
                            model.QueueDeclare(queueName,
                                false,
                                false,
                                false,
                                null);
                            model.QueueBind(queueName, exchangeName, subscribeKey, null);
                            var consumer = new AsyncEventingBasicConsumer(model);
                            consumer.Received += (sender, @event) =>
                                AsyncEventingBasicConsumerOnReceived(sender, @event, x);
                            model.BasicConsume(queueName, false, consumer);
                            return model;
                        });
            }
        }

        private Task AsyncEventingBasicConsumerOnReceived(object sender,
            BasicDeliverEventArgs @event,
            IClaptrapDesign minionDesign)
        {
            var consumer = (IAsyncBasicConsumer) sender;
            consumer.Model.BasicAck(@event.DeliveryTag, false);
            var payload = Decompress(@event);
            var data = _messageSerializer.Deserialize(payload);
            var evt = _eventStringSerializer.Deserialize(data);
            var minionId = new ClaptrapIdentity(evt.ClaptrapIdentity.Id, minionDesign.ClaptrapTypeCode);
            var minionProxy = _minionLocator.CreateProxy(minionId);
            return minionProxy.MasterEventReceivedAsync(new[] {evt});
        }

        private static byte[] Decompress(BasicDeliverEventArgs args)
        {
            var data = args.Body.ToArray();
            return args.BasicProperties.ContentEncoding switch
            {
                DeflateStreamHelper.ContentEncoding => DeflateStreamHelper.Decompress(data),
                GzipStreamHelper.ContentEncoding => GzipStreamHelper.Decompress(data),
                _ => data
            };
        }

        public Task CloseAsync()
        {
            if (_consumers != null)
            {
                foreach (var (_, model) in _consumers)
                {
                    model.Dispose();
                }
            }

            return Task.CompletedTask;
        }
    }
}