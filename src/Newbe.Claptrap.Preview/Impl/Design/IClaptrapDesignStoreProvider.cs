using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Design
{
    public interface IClaptrapDesignStoreProvider
    {
        IClaptrapDesignStore Create();
    }
}