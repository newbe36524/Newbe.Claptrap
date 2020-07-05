using System;

namespace Newbe.Claptrap
{
    public interface IBatchOperatorContainer
    {
        IBatchOperator GetOrAdd(IBatchOperatorKey key, Func<IBatchOperator> factory);
    }
}