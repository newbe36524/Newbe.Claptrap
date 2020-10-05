using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    public class AutoScaleAutoFlushListOptions : IAutoFlushListOptions
    {
        public delegate AutoScaleAutoFlushListOptions Factory(int initSize,
            TimeSpan debounceTime,
            int? minSize,
            int? maxSize);

        private readonly TimeSpan _debounceTime;
        private readonly ILogger<AutoScaleAutoFlushListOptions> _logger;
        private readonly int _initSize;
        private readonly int _minSize;
        private readonly int _maxSize;
        private readonly double _scaleUpRate;
        private readonly double _scaleDownRate;
        private readonly double _scaleUpConditionRate;
        private readonly double _scaleDownConditionRate;
        private int _currentSize;
        private int _lastPushCount;

        public AutoScaleAutoFlushListOptions(
            int initSize,
            TimeSpan debounceTime,
            int? minSize,
            int? maxSize,
            ILogger<AutoScaleAutoFlushListOptions> logger,
            double scaleUpRate = 2,
            double scaleDownRate = 0.9,
            double scaleUpConditionRate = 0.3,
            double scaleDownConditionRate = 0.3)
        {
            _currentSize = initSize;
            _debounceTime = debounceTime;
            _logger = logger;
            _initSize = initSize;
            _minSize = minSize ?? 100;
            _maxSize = maxSize ?? 200_000;
            _scaleUpRate = scaleUpRate;
            _scaleDownRate = scaleDownRate;
            _scaleUpConditionRate = scaleUpConditionRate;
            _scaleDownConditionRate = scaleDownConditionRate;
        }

        public int GetSize()
        {
            return _currentSize;
        }

        public TimeSpan GetDebounceTime()
        {
            return _debounceTime;
        }

        private int _scaleLock = 0;

        public void SetLastFlushCount(int lastPushCount)
        {
            const int locked = 1;
            const int unlocked = 0;
            if (unlocked == Interlocked.CompareExchange(ref _scaleLock, locked, unlocked))
            {
                try
                {
                    _lastPushCount = lastPushCount;
                    var leftCount = _currentSize - _lastPushCount;
                    var scaleUp = _currentSize * _scaleUpConditionRate;
                    var scaleDown = _currentSize * _scaleDownConditionRate;
                    var nextSize = 0;
                    if (leftCount > scaleDown)
                    {
                        nextSize = (int) (_currentSize * _scaleDownRate);
                    }
                    else if (leftCount < scaleUp)
                    {
                        nextSize = (int) (_currentSize * _scaleUpRate);
                    }
                    else
                    {
                        nextSize = _currentSize;
                    }

                    nextSize = Math.Max(nextSize, _minSize);
                    nextSize = Math.Min(nextSize, _maxSize);
                    if (_currentSize != nextSize)
                    {
                        _logger.LogInformation("auto scale buffer size {currentSize} -> {nextSize}",
                            _currentSize,
                            nextSize);
                        _currentSize = nextSize;
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _scaleLock, unlocked);
                }
            }
        }
    }
}