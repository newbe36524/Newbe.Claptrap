using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Design
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