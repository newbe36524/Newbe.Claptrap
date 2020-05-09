namespace Newbe.Claptrap.Design
{
    public interface IClaptrapDesignStoreValidator
    {
        (bool isOk, string errorMessage) Validate(IClaptrapDesignStore claptrapDesignStore);
    }
}