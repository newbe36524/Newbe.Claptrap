using System;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public interface IBatchOperatorContainer
    {
        IBatchOperator GetOrAdd(IBatchOperatorKey key, Func<IBatchOperator> factory);
    }
}