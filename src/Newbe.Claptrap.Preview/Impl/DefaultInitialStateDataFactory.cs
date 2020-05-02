using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl
{
    public class DefaultInitialStateDataFactory : IInitialStateDataFactory
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public DefaultInitialStateDataFactory(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public Task<IStateData> Create(IClaptrapIdentity identity)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            var findStateDataType = design.StateDataType;
            var stateData = (IStateData) Activator.CreateInstance(findStateDataType);
            return Task.FromResult(stateData);
        }
    }
}