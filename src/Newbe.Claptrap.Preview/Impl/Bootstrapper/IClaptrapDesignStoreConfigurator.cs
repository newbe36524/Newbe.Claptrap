using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IClaptrapDesignStoreConfigurator
    {
        void Configure(IClaptrapDesignStore designStore);
    }
}