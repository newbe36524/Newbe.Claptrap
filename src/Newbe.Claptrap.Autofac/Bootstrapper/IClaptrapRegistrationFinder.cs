using System.Collections.Generic;
using System.Reflection;

namespace Newbe.Claptrap.Autofac
{
    public interface IClaptrapRegistrationFinder
    {
        ClaptrapRegistration Find(IEnumerable<Assembly> assemblies);
    }
}