using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IClaptrapDesignStoreConfigurator
    {
        void Configure(IClaptrapDesignStore designStore);
    }
}