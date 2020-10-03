using System;
using System.Buffers;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newbe.Claptrap.MultipleChannelBatchOperators;

namespace Newbe.Claptrap
{
    public class ManualBatchOperator<T> : IBatchOperator<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ObjectPool<MultipleChannelBatchOperator<T>.BatchItem> _itemPool;

        public delegate ManualBatchOperator<T> Factory(BatchOperatorOptions<T> options);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly TaskConsumer<T> _taskConsumer;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ResultSetter<T> _resultSetter;
        private readonly ManualResetEvent _readerEvent;
        private readonly ManualResetEvent _writerEvent;
        private readonly ArrayPool<MultipleChannelBatchOperator<T>.BatchItem> _arrayPool;

        public ManualBatchOperator(
            BatchOperatorOptions<T> options,
            ObjectPool<MultipleChannelBatchOperator<T>.BatchItem> itemPool,
            ILogger<MultipleChannelBatchOperator<T>> logger)
        {
            if (options.BufferTime == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
            _itemPool = itemPool;
            _readerEvent = new ManualResetEvent(false);
            _writerEvent = new ManualResetEvent(true);
            _arrayPool = ArrayPool<MultipleChannelBatchOperator<T>.BatchItem>.Shared;
            _target = _arrayPool.Rent(_options.BufferCount!.Value);
            _itemsChannel = Channel.CreateUnbounded<MultipleChannelBatchOperator<T>.BatchItems>();
            var resultChannel = Channel.CreateUnbounded<MultipleChannelBatchOperator<T>.BatchItems>();

            _task = Task.Factory.StartNew(Pack, TaskCreationOptions.LongRunning).Unwrap();
            _taskConsumer = new TaskConsumer<T>(
                options,
                logger,
                _itemsChannel,
                resultChannel);
            _taskConsumer.Start();

            _resultSetter = new ResultSetter<T>(
                logger,
                itemPool,
                resultChannel);
            _resultSetter.Start();
        }

        private int _index = -1;
        private MultipleChannelBatchOperator<T>.BatchItem[] _target;
        private readonly Channel<MultipleChannelBatchOperator<T>.BatchItems> _itemsChannel;
        private Task _task;

        private async Task Pack()
        {
            while (true)
            {
                _readerEvent.WaitOne();
                if (_index != _options.BufferCount!.Value - 1)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                }

                _writerEvent.Reset();
                var length = _index + 1;
                var count = Math.Min(length, _options.BufferCount!.Value);
                var newBatchItems = _arrayPool.Rent(count);
                _target[..count].CopyTo(newBatchItems.AsSpan()[..count]);
                await _itemsChannel.Writer.WriteAsync(new MultipleChannelBatchOperator<T>.BatchItems
                {
                    Items = newBatchItems,
                    ItemCount = count
                });
                Interlocked.Exchange(ref _index, -1);
                _readerEvent.Reset();
                _writerEvent.Set();
            }
        }

        public ValueTask CreateTask(T input)
        {
            var item = _itemPool.Get();
            item.Input = input;
            var flag = true;
            do
            {
                _writerEvent.WaitOne();
                var increment = Interlocked.Increment(ref _index);
                if (increment >= _options.BufferCount!.Value)
                {
                    _writerEvent.Reset();
                    _readerEvent.Set();
                }
                else
                {
                    _target[increment] = item;
                    _readerEvent.Set();
                    flag = false;
                }
            } while (flag);

            return new ValueTask(item.Vts, item.Vts.Version);
        }
    }
}