using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Bootstrapper
{
    public interface IClaptrapDesignStoreProvider
    {
        IClaptrapDesignStore Create();
    }
}