using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    public class AutoFlushList<T>
    {
        public delegate AutoFlushList<T> Factory(IAutoFlushListOptions autoFlushListOptions,
            IConcurrentListPool<T> concurrentListPool,
            Func<ConcurrentList<T>, Task> flushFunc);

        private readonly IAutoFlushListOptions _autoFlushListOptions;
        private readonly IConcurrentListPool<T> _concurrentListPool;
        private readonly Func<ConcurrentList<T>, Task> _flushFunc;
        private readonly ILogger<AutoFlushList<T>> _logger;
        private volatile ConcurrentList<T> _currentBuffer;
        private readonly ManualResetEventSlim _bufferLock;
        private readonly ManualResetEventSlim _writeLock;
        private readonly ManualResetEventSlim _fullLock;

        // ReSharper disable once NotAccessedField.Local
        private readonly Task _task;

        // ReSharper disable once NotAccessedField.Local
        private readonly Task _task2;
        private int _size;

        public AutoFlushList(
            IAutoFlushListOptions autoFlushListOptions,
            IConcurrentListPool<T> concurrentListPool,
            Func<ConcurrentList<T>, Task> flushFunc,
            ILogger<AutoFlushList<T>> logger)
        {
            _autoFlushListOptions = autoFlushListOptions;
            _concurrentListPool = concurrentListPool;
            _flushFunc = flushFunc;
            _logger = logger;
            _size = _autoFlushListOptions.GetSize();
            _currentBuffer = concurrentListPool.Get(_size);
            _bufferLock = new ManualResetEventSlim(false);
            _writeLock = new ManualResetEventSlim(true);
            _fullLock = new ManualResetEventSlim(false);
            _task = Task.Factory.StartNew(SwitchBufferIfFull, TaskCreationOptions.LongRunning).Unwrap();
            _task2 = Task.Factory.StartNew(SwitchBufferWhileTimeout, TaskCreationOptions.LongRunning).Unwrap();
        }

        public async ValueTask Push(T item)
        {
            try
            {
                do
                {
                    var tryAdd = _currentBuffer.TryAdd(item, out var index);
                    if (tryAdd)
                    {
                        _bufferLock.Set();
                        return;
                    }

                    _bufferLock.Set();
                    await Task.Yield();
                } while (true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error while push data concurrent list");
            }
        }

        private async Task SwitchBufferIfFull()
        {
            while (true)
            {
                _fullLock.Wait();
                try
                {
                    if (_currentBuffer.IsFull)
                    {
                        _writeLock.Reset();
                        await SwitchBufferCore();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "error while change buffer");
                }
                finally
                {
                    _fullLock.Reset();
                    _writeLock.Set();
                }
            }
        }

        private async Task SwitchBufferWhileTimeout()
        {
            while (true)
            {
                _bufferLock.Wait();
                _fullLock.Set();
                try
                {
                    await Task.Delay(_autoFlushListOptions.GetDebounceTime());
                    _writeLock.Reset();
                    await SwitchBufferCore();
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "error while change buffer");
                }
                finally
                {
                    _bufferLock.Reset();
                    _writeLock.Set();
                }
            }
        }

        private int _bufferLocker = 0;

        private async Task SwitchBufferCore()
        {
            const int locked = 1;
            const int unlocked = 0;
            if (unlocked == Interlocked.CompareExchange(ref _bufferLocker, locked, unlocked))
            {
                try
                {
                    var now = _currentBuffer;
                    _autoFlushListOptions.SetLastFlushCount(now.Count);
                    _size = _autoFlushListOptions.GetSize();
                    _currentBuffer = _concurrentListPool.Get(_size);
                    await _flushFunc.Invoke(now);
                }
                finally
                {
                    Interlocked.Exchange(ref _bufferLocker, unlocked);
                }
            }
        }
    }
}