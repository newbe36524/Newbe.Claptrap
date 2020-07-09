using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Design
{
    public interface IClaptrapDesignStoreFactory
    {
        IClaptrapDesignStore Create(IEnumerable<Type> types);

        IClaptrapDesignStoreFactory AddProvider(IClaptrapDesignStoreProvider designStoreProvider);
    }
}