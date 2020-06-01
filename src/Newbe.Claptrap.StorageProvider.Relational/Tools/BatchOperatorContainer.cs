using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public class BatchOperatorContainer : IBatchOperatorContainer
    {
        private readonly Dictionary<string, IBatchOperator> _operators
            = new Dictionary<string, IBatchOperator>();

        private readonly object _locker = new object();

        public IBatchOperator GetOrAdd(IBatchOperatorKey key, Func<IBatchOperator> factory)
        {
            var stringKey = key.AsStringKey();
            // ReSharper disable once InconsistentlySynchronizedField
            if (_operators.ContainsKey(stringKey))
            {
                // ReSharper disable once InconsistentlySynchronizedField
                return _operators[stringKey];
            }

            lock (_locker)
            {
                if (_operators.ContainsKey(stringKey))
                {
                    return _operators[stringKey];
                }

                var batchOperator = factory.Invoke();
                _operators[stringKey] = batchOperator;
                return batchOperator;
            }
        }
    }
}