using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public class StaticClaptrapDesignStoreProvider : IClaptrapDesignStoreProvider
    {
        private readonly IEnumerable<IClaptrapDesign> _designs;

        public StaticClaptrapDesignStoreProvider(
            IEnumerable<IClaptrapDesign> designs)
        {
            _designs = designs;
        }

        public IClaptrapDesignStore Create()
        {
            var claptrapDesignStore = new ClaptrapDesignStore();
            foreach (var claptrapDesign in _designs)
            {
                claptrapDesignStore.AddOrReplace(claptrapDesign);
            }

            return claptrapDesignStore;
        }
    }
}