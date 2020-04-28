using System.Collections.Generic;

namespace Newbe.Claptrap.Preview
{
    public interface IActorTypeRegistrationCombiner
    {
        ClaptrapRegistration Combine(IEnumerable<ClaptrapRegistration> claptrapRegistrations);
    }
}