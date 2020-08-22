using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class MQSubscriberManager : IMQSubscriberManager
    {
        private readonly ILogger<MQSubscriberManager> _logger;
        private readonly IConnectionManager _connectionManager;
        private readonly IEventStringSerializer _eventStringSerializer;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMinionLocator _minionLocator;
        private IReadOnlyDictionary<string, IModel>? _consumers;

        public MQSubscriberManager(
            ILogger<MQSubscriberManager> logger,
            IConnectionManager connectionManager,
            IEventStringSerializer eventStringSerializer,
            IClaptrapDesignStore claptrapDesignStore,
            IMessageSerializer messageSerializer,
            IMinionLocator minionLocator)
        {
            _logger = logger;
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
                            _logger.LogTrace("declare exchange : {exchangeName}", exchangeName);
                            model.QueueDeclare(queueName,
                                false,
                                false,
                                false,
                                null);
                            _logger.LogTrace("declare queue : {queueName}", queueName);
                            model.QueueBind(queueName, exchangeName, subscribeKey, null);
                            _logger.LogTrace("bind queue : {queueName} to {exchangeName}", queueName, exchangeName);
                            var consumer = new AsyncEventingBasicConsumer(model);
                            consumer.Received += (sender, @event) =>
                                AsyncEventingBasicConsumerOnReceived(sender, @event, x);
                            model.BasicConsume(queueName, false, consumer);
                            return model;
                        });
            }
        }

        private async Task AsyncEventingBasicConsumerOnReceived(object sender,
            BasicDeliverEventArgs args,
            IClaptrapDesign minionDesign)
        {
            _logger.LogDebug("message received from rabbit mq, exchange : {exchange} ,routeKey : {routeKey}",
                args.Exchange,
                args.RoutingKey);
            var consumer = (IAsyncBasicConsumer) sender;
            consumer.Model.BasicAck(args.DeliveryTag, false);
            var payload = Decompress(args);
            var data = _messageSerializer.Deserialize(payload);
            var evt = _eventStringSerializer.Deserialize(data);
            var minionId = new ClaptrapIdentity(evt.ClaptrapIdentity.Id, minionDesign.ClaptrapTypeCode);
            var minionProxy = _minionLocator.CreateProxy(minionId);
            _logger.LogTrace("create minion proxy for {id}", minionId);
            await minionProxy.MasterEventReceivedAsync(new[] {evt});
            _logger.LogDebug("a message sent to minion {minionId}", minionId);
        }

        private static ReadOnlyMemory<byte> Decompress(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.ContentEncoding switch
            {
                DeflateStreamHelper.ContentEncoding => DeflateStreamHelper.Decompress(args.Body),
                GzipStreamHelper.ContentEncoding => GzipStreamHelper.Decompress(args.Body),
                _ => args.Body
            };
        }

        public Task CloseAsync()
        {
            if (_consumers != null)
            {
                foreach (var (_, model) in _consumers)
                {
                    model.Close();
                    model.Dispose();
                }
            }

            return Task.CompletedTask;
        }
    }
}