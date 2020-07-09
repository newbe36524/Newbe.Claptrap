using System.Collections.Generic;

namespace Newbe.Claptrap.Design
{
    public interface IClaptrapDesignStoreCombiner
    {
        IClaptrapDesignStore Combine(IEnumerable<IClaptrapDesignStore> stores);
    }
}