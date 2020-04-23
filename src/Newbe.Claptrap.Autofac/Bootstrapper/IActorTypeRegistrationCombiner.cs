using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public interface IActorTypeRegistrationCombiner
    {
        ClaptrapRegistration Combine(IEnumerable<ClaptrapRegistration> claptrapRegistrations);
    }
}