using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IClaptrapDesignStoreConfigurator
    {
        void Configurate(IClaptrapDesignStore designStore);
    }
}