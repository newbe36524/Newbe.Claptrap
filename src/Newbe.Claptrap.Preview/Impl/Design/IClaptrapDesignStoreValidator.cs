using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Design
{
    public interface IClaptrapDesignStoreValidator
    {
        (bool isOk, string errorMessage) Validate(IClaptrapDesignStore claptrapDesignStore);
    }
}