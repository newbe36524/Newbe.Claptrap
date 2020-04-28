using System.Collections.Generic;
using System.Reflection;

namespace Newbe.Claptrap.Preview
{
    public interface IClaptrapRegistrationFinder
    {
        ClaptrapRegistration Find(IEnumerable<Assembly> assemblies);
    }
}