using System.Buffers;
using Microsoft.Extensions.ObjectPool;

namespace Newbe.Claptrap
{
    public class ConcurrentListPool<T> : IConcurrentListPool<T>
    {
        private readonly ObjectPool<ConcurrentList<T>> _objectPool;
        private readonly ArrayPool<T> _arrayPool;

        public ConcurrentListPool(
            ObjectPool<ConcurrentList<T>> objectPool)
        {
            _objectPool = objectPool;
            _arrayPool = ArrayPool<T>.Shared;
        }

        public ConcurrentList<T> Get(int size)
        {
            var concurrentList = _objectPool.Get();
            concurrentList.Init(_arrayPool, size);
            return concurrentList;
        }

        public void Return(ConcurrentList<T> list)
        {
            list.Dispose();
            _objectPool.Return(list);
        }
    }
}