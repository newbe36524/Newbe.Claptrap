using System.Collections.Generic;

namespace Newbe.Claptrap.Design
{
    public interface IClaptrapDesignStoreValidator
    {
        (bool isOk, string errorMessage) Validate(IEnumerable<IClaptrapDesign> designs);
    }
}