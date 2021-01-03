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
            Action<ConcurrentList<T>> flushFunc);

        private readonly IAutoFlushListOptions _autoFlushListOptions;
        private readonly IConcurrentListPool<T> _concurrentListPool;
        private readonly Action<ConcurrentList<T>> _flushFunc;
        private readonly ILogger<AutoFlushList<T>> _logger;
        private volatile ConcurrentList<T> _currentBuffer;
        private readonly ManualResetEventSlim _bufferLock;
        private readonly ManualResetEventSlim _writeLock;
        private readonly ManualResetEventSlim _fullLock;

        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _switchBufferIfFullThread;

        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _switchBufferWhileTimeoutThread;

        private int _size;

        public AutoFlushList(
            IAutoFlushListOptions autoFlushListOptions,
            IConcurrentListPool<T> concurrentListPool,
            Action<ConcurrentList<T>> flushFunc,
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
            _switchBufferIfFullThread = new Thread(SwitchBufferIfFull)
            {
                IsBackground = true
            };
            _switchBufferIfFullThread.Start();
            _switchBufferWhileTimeoutThread = new Thread(SwitchBufferWhileTimeout)
            {
                IsBackground = true
            };
            _switchBufferWhileTimeoutThread.Start();
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

        private void SwitchBufferIfFull()
        {
            while (true)
            {
                _fullLock.Wait();
                try
                {
                    if (_currentBuffer.IsFull)
                    {
                        _writeLock.Reset();
                        SwitchBufferCore();
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

        private void SwitchBufferWhileTimeout()
        {
            while (true)
            {
                _bufferLock.Wait();
                _fullLock.Set();
                try
                {
                    Thread.Sleep(_autoFlushListOptions.GetDebounceTime());
                    _writeLock.Reset();
                    SwitchBufferCore();
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

        private void SwitchBufferCore()
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
                    _flushFunc.Invoke(now);
                }
                finally
                {
                    Interlocked.Exchange(ref _bufferLocker, unlocked);
                }
            }
        }
    }
}