using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl
{
    public class DefaultInitialStateDataFactory : IInitialStateDataFactory
    {
        private readonly IClaptrapDesignStoreAccessor _claptrapDesignStoreAccessor;

        public DefaultInitialStateDataFactory(
            IClaptrapDesignStoreAccessor claptrapDesignStoreAccessor)
        {
            _claptrapDesignStoreAccessor = claptrapDesignStoreAccessor;
        }

        public Task<IStateData> Create(IClaptrapIdentity identity)
        {
            var findStateDataType = _claptrapDesignStoreAccessor.FindStateDataType(identity.TypeCode);
            var stateData = (IStateData) Activator.CreateInstance(findStateDataType);
            return Task.FromResult(stateData);
        }
    }
}