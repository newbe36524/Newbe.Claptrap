using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class MqSender : IMQSender
    {
        public delegate MqSender Factory(string exchange,
            IReadOnlyDictionary<string, string> topics,
            IConnection connection);

        private readonly string _exchange;
        private readonly IReadOnlyDictionary<string, string> _topics;
        private readonly IConnection _connection;
        private readonly IEventStringSerializer _eventStringSerializer;
        private readonly IMessageSerializer _messageSerializer;
        private readonly ILogger<MqSender> _logger;
        private readonly IOptions<ClaptrapServerOptions> _options;
        private readonly IBatchOperator<IEvent> _batchOperator;

        public MqSender(
            string exchange,
            IReadOnlyDictionary<string, string> topics,
            IConnection connection,
            IEventStringSerializer eventStringSerializer,
            IMessageSerializer messageSerializer,
            BatchOperator<IEvent>.Factory batchOperatorFactory,
            IBatchOperatorContainer batchOperatorContainer,
            ILogger<MqSender> logger,
            IOptions<ClaptrapServerOptions> options)
        {
            _exchange = exchange;
            _topics = topics;
            _connection = connection;
            _eventStringSerializer = eventStringSerializer;
            _messageSerializer = messageSerializer;
            _logger = logger;
            _options = options;
            var operatorKey = new BatchOperatorKey()
                .With(nameof(MqSender))
                .With(exchange);

            _batchOperator = (IBatchOperator<IEvent>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<IEvent>
                    {
                        BufferTime = TimeSpan.FromMilliseconds(10),
                        BufferCount = 100,
                        DoManyFunc = (events, cacheData) => SendMany(events)
                    }));
        }

        private Task SendMany(IEnumerable<IEvent> entities)
        {
            return Task.Run(SendManyCode);

            void SendManyCode()
            {
                try
                {
                    SendCore();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "failed to send message to rabbit mq");
                }

                void SendCore()
                {
                    using var model = _connection.CreateModel();

                    var batch = model.CreateBasicPublishBatch();
                    var count = 0;
                    foreach (var @event in entities)
                    {
                        var eventString = _eventStringSerializer.Serialize(@event);
                        var bytes = _messageSerializer.Serialize(eventString);
                        var basicProperties = model.CreateBasicProperties();
                        basicProperties.ContentType = "application/json";

                        var compressType = _options.Value?.RabbitMQ?.CompressType ?? CompressType.None;
                        var (encoding, body) = CompressData(bytes, compressType);
                        if (compressType != CompressType.None)
                        {
                            _logger.LogTrace("message size : {from} -> {to} diif : {diff}",
                                bytes.Length,
                                body.Length,
                                bytes.Length - body.Length);
                        }

                        basicProperties.ContentEncoding = encoding;
                        batch.Add(_exchange, _topics[@event.EventTypeCode], true, basicProperties, body);
                        count++;
                    }

                    batch.Publish();
                    _logger.LogDebug("sent {count} message to rabbit mq", count);
                    model.Close();
                }
            }
        }

        private (string? encoding, ReadOnlyMemory<byte> data) CompressData(ReadOnlyMemory<byte> source,
            CompressType type)
        {
            _logger.LogTrace("compress type : {type}", type);
            if (type == CompressType.None)
            {
                return ("utf8", source);
            }

            switch (type)
            {
                case CompressType.GZip:
                    return (GzipStreamHelper.ContentEncoding, GzipStreamHelper.Compress(source));
                case CompressType.Deflate:
                    return (DeflateStreamHelper.ContentEncoding, DeflateStreamHelper.Compress(source));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public Task SendTopicAsync(IEvent @event)
        {
            return _batchOperator.CreateTask(@event);
        }
    }
}