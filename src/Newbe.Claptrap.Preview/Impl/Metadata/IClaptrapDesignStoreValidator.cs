using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public interface IClaptrapDesignStoreValidator
    {
        (bool isOk, string errorMessage) Validate(IClaptrapDesignStore claptrapDesignStore);
    }
}