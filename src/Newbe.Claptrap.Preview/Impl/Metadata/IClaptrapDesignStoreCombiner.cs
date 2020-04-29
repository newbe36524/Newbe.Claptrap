using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public interface IClaptrapDesignStoreCombiner
    {
        IClaptrapDesignStore Combine(IEnumerable<IClaptrapDesignStore> stores);
    }
}