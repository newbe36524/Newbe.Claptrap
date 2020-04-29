using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public interface IClaptrapDesignStoreProvider
    {
        IClaptrapDesignStore Create();
    }
}