using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Design
{
    public class ClaptrapDesignStoreCombiner : IClaptrapDesignStoreCombiner
    {
        private readonly ClaptrapDesignStore.Factory _claptrapDesignStoreFactory;

        public ClaptrapDesignStoreCombiner(
            ClaptrapDesignStore.Factory claptrapDesignStoreFactory)
        {
            _claptrapDesignStoreFactory = claptrapDesignStoreFactory;
        }

        public IClaptrapDesignStore Combine(IEnumerable<IClaptrapDesignStore> stores)
        {
            var re = _claptrapDesignStoreFactory.Invoke();
            foreach (var claptrapDesignStore in stores)
            {
                foreach (var claptrapDesign in claptrapDesignStore)
                {
                    re.AddOrReplace(claptrapDesign);
                }
            }

            return re;
        }
    }
}