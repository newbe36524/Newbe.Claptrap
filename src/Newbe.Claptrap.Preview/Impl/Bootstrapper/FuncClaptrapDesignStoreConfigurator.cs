using System;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class FuncClaptrapDesignStoreConfigurator : IClaptrapDesignStoreConfigurator
    {
        private readonly Action<IClaptrapDesignStore> _action;

        public FuncClaptrapDesignStoreConfigurator(
            Action<IClaptrapDesignStore> action)
        {
            _action = action;
        }

        public void Configure(IClaptrapDesignStore designStore)
        {
            _action.Invoke(designStore);
        }
    }
}