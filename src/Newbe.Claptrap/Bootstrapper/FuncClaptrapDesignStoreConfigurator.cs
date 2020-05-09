using System;
using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Bootstrapper
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