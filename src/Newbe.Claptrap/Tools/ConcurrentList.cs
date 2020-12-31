using System;
using System.Buffers;
using System.Threading;

namespace Newbe.Claptrap
{
    public class ConcurrentList<T> : IDisposable
    {
        /// <summary>
        /// Category, to specify what this list for 
        /// </summary>
        public string? Category { get; set; }

        private ArrayPool<T>? _arrayPool;
        private int _limit;
        private int _index = -1;
        private T[]? _buffer;

        public bool TryAdd(T item, out int index)
        {
            var newIndex = Interlocked.Increment(ref _index);
            if (newIndex >= _limit)
            {
                index = -1;
                return false;
            }

            index = newIndex;
            Buffer[newIndex] = item;
            return true;
        }

        public void ResetIndex()
        {
            if (_arrayPool == null)
            {
                throw new InvalidOperationException("you have to init list first");
            }

            _index = -1;
        }

        public void Init(ArrayPool<T> arrayPool, int size)
        {
            _arrayPool = arrayPool;
            Buffer = _arrayPool.Rent(size);
            _limit = size;
            ResetIndex();
        }

        public void Resize(int size)
        {
            if (_arrayPool == null)
            {
                throw new InvalidOperationException("you have to init list first");
            }

            _arrayPool.Return(Buffer);
            Buffer = _arrayPool.Rent(size);
            _limit = size;
            ResetIndex();
        }

        public int Index => _index;
        public int Count => Math.Min(_index + 1, _limit);

        public T[] Buffer
        {
            get => _buffer!;
            private set => _buffer = value;
        }

        public bool IsFull => _index >= _limit;

        public int Limit => _limit;

        public void Dispose()
        {
            if (_buffer != null)
            {
                _arrayPool?.Return(Buffer);
                _buffer = null;
            }
        }
    }
}