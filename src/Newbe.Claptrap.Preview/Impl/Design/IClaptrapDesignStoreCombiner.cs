using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Design
{
    public interface IClaptrapDesignStoreCombiner
    {
        IClaptrapDesignStore Combine(IEnumerable<IClaptrapDesignStore> stores);
    }
}