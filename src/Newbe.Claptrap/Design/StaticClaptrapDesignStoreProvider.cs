using System.Collections.Generic;

namespace Newbe.Claptrap.Design
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